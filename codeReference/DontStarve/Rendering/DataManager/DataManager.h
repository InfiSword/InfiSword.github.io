#pragma once

class DataManager : public CSingleTon<DataManager>
{
    friend class CSingleTon<DataManager>;
private:
    DataManager();
    ~DataManager();

public:
    void Init();
    void Release();

    const ResourcePathUtils::ObjectResourceDef* GetObjectResourceInfo(GameObjectID id) const;
    const std::map<GameObjectID, ResourcePathUtils::ObjectResourceDef>& GetAllObjectResources() const { return m_objectResources; }

private:
    std::map<GameObjectID, ResourcePathUtils::ObjectResourceDef> m_objectResources;
};
