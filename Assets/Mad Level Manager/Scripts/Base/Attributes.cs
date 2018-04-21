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

namespace MadLevelManager.Backend {

    public class Required : Attribute { }

    public class Optional : Attribute { }

    public class DisplayedName : Attribute {
        public readonly string name;

        public DisplayedName(string name) {
            this.name = name;
        }
    }

    public class HelpURL : Attribute {
        public readonly string url;

        public HelpURL(string url) {
            this.url = url;
        }
    }

}