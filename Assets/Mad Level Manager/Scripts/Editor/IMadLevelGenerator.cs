/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IMadLevelGenerator {

    string GetLevelName(int levelNumber);

    string GetLevelArguments(int levelNumber);

}