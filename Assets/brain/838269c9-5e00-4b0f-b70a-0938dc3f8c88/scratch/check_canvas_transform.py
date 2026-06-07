with open("Assets/Scenes/SampleScene.unity", 'r', encoding='utf-8', errors='ignore') as f:
    content = f.read()

docs = content.split('--- !u!')
for doc in docs:
    if "&1675998070" in doc:
        print(doc[:500])
        break
