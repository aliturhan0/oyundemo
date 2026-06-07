import re

# Read unity file
with open("Assets/Scenes/SampleScene.unity", 'r', encoding='utf-8', errors='ignore') as f:
    content = f.read()

# Separate documents
docs = content.split('--- !u!')

# Maps
go_names = {}
go_active = {}
comp_to_go = {}
rt_children = {}
rt_father = {}
go_to_rt = {}
images_raycast = {}

for doc in docs:
    doc = doc.strip()
    if not doc:
        continue
        
    lines = doc.split('\n')
    if len(lines) < 2:
        continue
        
    header = lines[0]
    class_name = lines[1].strip().replace(':', '')
    
    # Extract fileID
    m = re.search(r'&(\d+)', header)
    if not m:
        continue
    fileID = m.group(1)
    
    # 1. Parse GameObject
    if class_name == "GameObject":
        name_match = re.search(r'm_Name:\s*(.*)', doc)
        name = name_match.group(1).strip('"\' \r\n') if name_match else "Unnamed"
        active_match = re.search(r'm_IsActive:\s*(\d)', doc)
        active = active_match.group(1) == "1" if active_match else True
        
        go_names[fileID] = name
        go_active[fileID] = active
        
    # 2. Parse Transform / RectTransform
    elif class_name in ["RectTransform", "Transform"]:
        go_match = re.search(r'm_GameObject:\s*\{\s*fileID:\s*(\d+)\s*\}', doc)
        father_match = re.search(r'm_Father:\s*\{\s*fileID:\s*(\d+)\s*\}', doc)
        
        go = go_match.group(1) if go_match else None
        father = father_match.group(1) if father_match else None
        
        if go:
            comp_to_go[fileID] = go
            go_to_rt[go] = fileID
            if father:
                rt_father[fileID] = father
                
            # Parse children
            children = []
            children_section = re.search(r'm_Children:\s*\n((?:\s*-\s*\{\s*fileID:\s*\d+\s*\}\s*\n?)*)', doc)
            if children_section:
                child_ids = re.findall(r'fileID:\s*(\d+)', children_section.group(1))
                children = child_ids
            rt_children[fileID] = children

    # 3. Parse Image (MonoBehaviour with m_RaycastTarget)
    elif class_name == "MonoBehaviour":
        # Check if it has m_RaycastTarget
        if "m_RaycastTarget:" in doc:
            go_match = re.search(r'm_GameObject:\s*\{\s*fileID:\s*(\d+)\s*\}', doc)
            if go_match:
                go = go_match.group(1)
                raycast_match = re.search(r'm_RaycastTarget:\s*(\d)', doc)
                raycast = raycast_match.group(1) == "1" if raycast_match else True
                images_raycast[go] = raycast

# Trace Canvas children
canvas_go = None
for go_id, name in go_names.items():
    if name == "Canvas":
        canvas_go = go_id
        break

if not canvas_go:
    print("Canvas GameObject not found!")
else:
    print(f"Canvas (fileID: {canvas_go}) found!")
    canvas_rt = go_to_rt.get(canvas_go)
    if not canvas_rt:
        print("Canvas has no RectTransform!")
    else:
        children_rt = rt_children.get(canvas_rt, [])
        print(f"Total children under Canvas: {len(children_rt)}")
        
        def print_node(rt_id, depth=0):
            go_id = comp_to_go.get(rt_id)
            if not go_id:
                return
            name = go_names.get(go_id, "Unknown")
            active = go_active.get(go_id, True)
            raycast = images_raycast.get(go_id, None)
            raycast_str = f" | RaycastTarget: {raycast}" if raycast is not None else ""
            print("  " * depth + f"- '{name}' (Active: {active}{raycast_str})")
            
            # Print children
            for child_rt in rt_children.get(rt_id, []):
                print_node(child_rt, depth + 1)
                
        for child_rt in children_rt:
            print_node(child_rt, 1)
