import re

def find_button_and_parent_yaml(filepath):
    with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()
        
    docs = content.split('--- !u!')
    
    # 1. Find "Sonraki Bölüm" GameObject fileID
    button_go_id = None
    for doc in docs:
        if "GameObject:" in doc and "Sonraki" in doc:
            first_line = doc.split('\n')[0]
            m = re.search(r'&(\d+)', first_line)
            if m:
                button_go_id = m.group(1)
                print(f"Found Button GameObject ID: {button_go_id}")
                print(doc[:300])
                break
                
    if not button_go_id:
        print("Button GameObject not found!")
        return

    # 2. Find RectTransform, Button, UIStyler and other components for this GameObject
    button_rt_id = None
    button_father_rt_id = None
    
    for doc in docs:
        if f"m_GameObject: {{fileID: {button_go_id}}}" in doc:
            first_line = doc.split('\n')[0]
            m_type = doc.split('\n')[1]
            m_id = re.search(r'&(\d+)', first_line).group(1) if re.search(r'&(\d+)', first_line) else "unknown"
            print(f"\n--- Component {m_type.strip()} (fileID: {m_id}) ---")
            print(doc[:500])
            
            if "RectTransform:" in doc:
                button_rt_id = m_id
                father_match = re.search(r'm_Father:\s*\{\s*fileID:\s*(\d+)\s*\}', doc)
                if father_match:
                    button_father_rt_id = father_match.group(1)
                    print(f"Father RectTransform ID: {button_father_rt_id}")

    # 3. Find parent GameObject and its RectTransform
    if button_father_rt_id:
        parent_go_id = None
        for doc in docs:
            if "RectTransform:" in doc:
                first_line = doc.split('\n')[0]
                m_id = re.search(r'&(\d+)', first_line).group(1) if re.search(r'&(\d+)', first_line) else "unknown"
                if m_id == button_father_rt_id:
                    go_match = re.search(r'm_GameObject:\s*\{\s*fileID:\s*(\d+)\s*\}', doc)
                    if go_match:
                        parent_go_id = go_match.group(1)
                        print(f"\nFound Father RectTransform. Father GameObject ID: {parent_go_id}")
                        print(doc[:300])
                        break
                        
        if parent_go_id:
            for doc in docs:
                first_line = doc.split('\n')[0]
                m_id = re.search(r'&(\d+)', first_line).group(1) if re.search(r'&(\d+)', first_line) else "unknown"
                if m_id == parent_go_id:
                    print(f"\n--- Father GameObject (fileID: {m_id}) ---")
                    print(doc[:300])
                elif f"m_GameObject: {{fileID: {parent_go_id}}}" in doc:
                    m_type = doc.split('\n')[1]
                    print(f"\n--- Father Component {m_type.strip()} (fileID: {m_id}) ---")
                    print(doc[:500])

find_button_and_parent_yaml("Assets/Scenes/SampleScene.unity")
