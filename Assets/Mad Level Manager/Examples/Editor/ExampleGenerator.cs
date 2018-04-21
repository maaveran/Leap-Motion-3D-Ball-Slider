/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class ExampleGenerator : IMadLevelGenerator {
    public string GetLevelName(int levelNumber) {
        return "Generated Level " + levelNumber;
    }

    public string GetLevelArguments(int levelNumber) {
        return "arguments for " + levelNumber;
    }
}

#if !UNITY_3_5
} // namespace
#endif