with open("Assets/Scenes/SampleScene.unity", 'r', encoding='utf-8', errors='ignore') as f:
    content = f.read()

docs = content.split('--- !u!')
for doc in docs:
    if "&1675998070" in doc:
        print("FOUND DOC:")
        print(repr(doc[:200]))
        import re
        go_match = re.search(r'm_GameObject:\s*\{\s*fileID:\s*(\d+)\s*\}', doc)
        print("go_match:", go_match)
        if go_match:
            print("go_id:", go_match.group(1))
        break
