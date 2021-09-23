using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QueueConnect.Development
{
    public class Test : MonoBehaviour
    {
        private readonly List<TestObjects> testObjects = new List<TestObjects>();

        void Start()
        {
            var _maxChance = (uint)testObjects.Sum(_TestObject => _TestObject.chance);
            var _randomNumber = Random.Range(1, _maxChance);
            var _chance = 0;

            for (int i = 0; i < testObjects.Count; i++)
            {
                if (_randomNumber <= testObjects[i].chance + _chance)
                {
                    // Do stuff
                    break;
                }

                _chance += (int)testObjects[i].chance;
            }
        }

        private class TestObjects
        {
            public readonly uint chance;

            public TestObjects(uint _Chance)
            {
                this.chance = _Chance;
            }
        }
    }   
}