using System;
using Ganymed.Utils.ExtensionMethods;
using QueueConnect.Plugins.Ganymed.Utils.Scripts.ExtensionMethods;
using UnityEngine;

namespace QueueConnect.StatSystem
{
    public readonly struct Stat
    {
        private const string intFormat = "00";
        private const string floatFormat = "0.00";

        public readonly string name;
        public readonly string value;
        public readonly Color color;

        public readonly bool isEmpty;
        
        public Stat(string name, string value, Color? color = null)
        {
            this.name = name.Dehumanize();
            this.value = value;
            this.color = color ?? Color.white;
            isEmpty = false;
        }
        
        public Stat(string name, int value, Color? color = null)
        {
            this.name = name.Dehumanize();
            this.value = value.ToString(intFormat);
            this.color = color ?? Color.white;
            isEmpty = false;
        }

        
        public Stat(string name, float value, Color? color = null)
        {
            this.name = name.Dehumanize();
            this.value = $"{value.ToString(floatFormat)}{(name.Contains("time")? "s" : "")}";
            this.color = color ?? Color.white;
            isEmpty = false;
        }

        private Stat(bool empty)
        {
            isEmpty = true;
            this.name = null;
            this.value = null;
            this.color = default;
        }

        public static readonly Stat Empty = new Stat(true);
    }
}