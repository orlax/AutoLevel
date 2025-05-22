using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IlevelBuilderInfoGetter
{
    public int uid { get; set; }
    public void GetLevelBuilderInfo(AutoLevel.LevelBuilderInfo info, AutoLevelLine autoLevel_);
    public void SageLevelBuilderInfo();
}
