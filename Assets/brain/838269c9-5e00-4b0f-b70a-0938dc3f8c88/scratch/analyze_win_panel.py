import re

def parse_unity_file(filepath):
    with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()
    
    docs = content.split('--- !u!')
    
    gameObjects = {}
    rectTransforms = {}
    monoBehaviours = {}
    
    for doc in docs:
        if not doc.strip():
            continue
        
        first_line = doc.split('\n')[0]
        match = re.match(r'(\d+)\s+&(\d+)', first_line)
        if not match:
            match = re.match(r'(\d+)', first_line)
            if not match:
                continue
            fileID = match.group(1)
        else:
            fileID = match.group(2)
            
        if "GameObject:" in doc:
            name_match = re.search(r'm_Name:\s*(.*)', doc)
            name = name_match.group(1).strip() if name_match else "Unnamed"
            name = name.strip('"\'')
            active_match = re.search(r'm_IsActive:\s*(\d)', doc)
            active = active_match.group(1) == "1" if active_match else True
            gameObjects[fileID] = {"name": name, "active": active, "components": []}
            
        elif "RectTransform:" in doc:
            father_match = re.search(r'm_Father:\s*\{\s*fileID:\s*(\d+)\s*\}', doc)
            father = father_match.group(1) if father_match else None
            go_match = re.search(r'm_GameObject:\s*\{\s*fileID:\s*(\d+)\s*\}', doc)
            go = go_match.group(1) if go_match else None
            
            # Get child fileIDs
            children = []
            children_section = re.search(r'm_Children:\s*\n((?:\s*-\s*\{\s*fileID:\s*\d+\s*\}\s*\n?)*)', doc)
            if children_section:
                child_ids = re.findall(r'fileID:\s*(\d+)', children_section.group(1))
                children = child_ids
                
            rectTransforms[fileID] = {"father": father, "gameObject": go, "children": children}
            
        elif "MonoBehaviour:" in doc:
            go_match = re.search(r'm_GameObject:\s*\{\s*fileID:\s*(\d+)\s*\}', doc)
            go = go_match.group(1) if go_match else None
            enabled_match = re.search(r'm_Enabled:\s*(\d)', doc)
            enabled = enabled_match.group(1) == "1" if enabled_match else True
            
            if go:
                monoBehaviours.setdefault(go, []).append({
                    "fileID": fileID,
                    "enabled": enabled,
                    "content": doc
                })

    # Map RectTransforms to GameObjects
    rt_to_go = {}
    go_to_rt = {}
    for rt_id, rt in rectTransforms.items():
        go_id = rt["gameObject"]
        if go_id:
            rt_to_go[rt_id] = go_id
            go_to_rt[go_id] = rt_id

    # Print all objects of interest
    print("ALL BUTTONS, PANELS AND THEIR COMPONENTS:")
    for go_id, go in gameObjects.items():
        name = go["name"]
        if any(x in name.lower() for x in ["panel", "button", "canvas", "sonraki", "next"]):
            rt_id = go_to_rt.get(go_id)
            parent_name = "None"
            if rt_id:
                father_rt_id = rectTransforms[rt_id]["father"]
                if father_rt_id in rt_to_go:
                    parent_name = gameObjects[rt_to_go[father_rt_id]]["name"]
                    
            mb_list = monoBehaviours.get(go_id, [])
            comps = []
            for mb in mb_list:
                if "UIStyler" in mb["content"]:
                    comps.append("UIStyler")
                elif "Button" in mb["content"]:
                    interactable_match = re.search(r'm_Interactable:\s*(\d)', mb["content"])
                    interactable = interactable_match.group(1) == "1" if interactable_match else True
                    comps.append(f"Button(interactable={interactable})")
                elif "GraphicRaycaster" in mb["content"]:
                    comps.append("GraphicRaycaster")
                elif "Canvas" in mb["content"] and "CanvasRenderer" not in mb["content"]:
                    comps.append("Canvas")
                elif "CanvasGroup" in mb["content"]:
                    comps.append("CanvasGroup")
            
            print(f"Name: '{name}' | Active: {go['active']} | Parent: '{parent_name}' | Comps: {comps}")

parse_unity_file("Assets/Scenes/SampleScene.unity")
