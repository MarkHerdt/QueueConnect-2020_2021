using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QueueConnect.GameSystem
{
    /// <summary>
    /// Object Pool for GameObjects
    /// </summary>
    public class ObjectPool
    {
        #region Privates
            // Object
            private readonly GameObject prefab;
            private ObjectAndType typeObject;
            private readonly Component componentType;
            private readonly Action<ObjectPool, GameObject> callBack;
            // Position
            private readonly Transform parent;
            private readonly Vector3 origin;
            // Lists
            private readonly List<ObjectAndType> allObjects = new List<ObjectAndType>();
            private readonly Queue<ObjectAndType> freeObjects = new Queue<ObjectAndType>();
            private readonly List<ObjectAndType> objectsInUse = new List<ObjectAndType>();
        #endregion

        #region Properties
            /// <summary>
            /// All objects in the Pool
            /// </summary>
            public IEnumerable<ObjectAndType> AllObjects => allObjects;
            /// <summary>
            /// All objects in the Pool that are currently not in use
            /// </summary>
            public IEnumerable<ObjectAndType> FreeObjects => freeObjects;
            /// <summary>
            /// All objects in the Pool that are currently being used
            /// </summary>
            public IEnumerable<ObjectAndType> ObjectsInUse => objectsInUse;
        #endregion
        
        /// <summary>
        /// Creates a new Object Pool
        /// </summary>
        /// <param name="_Prefab">Prefab to Instantiate</param>
        /// <param name="_Parent">Parent Object, the Instantiated GameObjects should be children of</param>
        /// <param name="_Origin">Position of Objects when they're activated (uses "prefab.transform.position" if null)</param>
        /// <param name="_CallBack">Event that is broadcasted when an Object is instantiated</param>
        /// <param name="_ComponentType">
        /// Additional Component you want to have access from the Pool <br/>
        /// <b>Component has to be on the root Prefab Object!</b>
        /// </param>
        public ObjectPool(GameObject _Prefab, Transform _Parent = null, Vector3? _Origin = null, Action<ObjectPool, GameObject> _CallBack = null, Component _ComponentType = null) 
        {
            this.prefab = _Prefab;
            this.parent = _Parent;
            this.callBack = _CallBack;
            this.componentType = _ComponentType;

            if (_Origin != null)
            {
                var _x = _Origin.Value.x;
                var _y = _Origin.Value.y;
                var _z = _Origin.Value.z;

                this.origin = new Vector3(_x, _y, _z);
            }
            else
            {
                this.origin = _Prefab.transform.position;
            }
        }

        /// <summary>
        /// Is fired when an ObjectPool had no free Objects left and needed to instantiate an additional Object
        /// </summary>
        public static event Action<ObjectPool> OnAdditionalObjectNeeded;

        /// <summary>
        /// Fires an event to tell all subscribers that the passed ObjectPool needed to instantiate a new Object
        /// </summary>
        /// <param name="_Pool">The ObjectPool that needed to instantiate a new Object</param>
        private static void AdditionalObjectNeeded(ObjectPool _Pool)
        {
            OnAdditionalObjectNeeded?.Invoke(_Pool);
        }

        /// <summary>
        /// Instantiates a new GameObject and adds it to the Queue
        /// </summary>
        /// <param name="_Amount">How many Objects to instantiate</param>
        /// <returns>Returns an Array with all Instantiated GameObjects</returns>
        public ObjectAndType[] AddObject(byte _Amount = 1)
        {
            Component _component = null;
            var _objects = new ObjectAndType[_Amount];

            for (byte i = 0; i < _Amount; i++)
            {
                // Deactivates the Prefab GameObject, so the "OnEnable"-Method is not called on Instantiation
                prefab.SetActive(false);

                var _gameObject = (parent != null ? Object.Instantiate(prefab, parent) : Object.Instantiate(prefab));
                if (componentType != null) { _component = _gameObject.GetComponent(componentType.GetType()); }
                
                typeObject = new ObjectAndType(_gameObject, _component);
                callBack?.Invoke(this, typeObject.GameObject);

                _objects[i] = typeObject;
                allObjects.Add(typeObject);
                freeObjects.Enqueue(typeObject);
            }

            return _objects;
        }

        /// <summary>
        /// Returns the GameObject first in Queue when it's not empty <br/>
        /// Instantiates a new GameObject and returns it if the Queue is empty
        /// </summary>
        /// <param name="_NewParent">Makes this GameObject a child of the passed Transform</param>
        /// <param name="_NewPosition">Position this GameObject should have when activated (uses "gameObject.transform.localPosition")</param>
        /// <param name="_GlobalPosition">If "true", uses "gameObject.transform.position"</param>
        /// <returns></returns>
        public ObjectAndType GetObject(Transform _NewParent = null, Vector3? _NewPosition = null, bool _GlobalPosition = false)
        {
            while (true)
            {
                if (freeObjects.Count > 0)
                {
                    // Gets the first Queue element
                    typeObject = freeObjects.Dequeue();
                    // Adds the removed Queue element to the list
                    objectsInUse.Add(typeObject);

                    if ((object)_NewParent != null)
                    {
                        typeObject.GameObject.transform.SetParent(_NewParent);
                    }

                    if (_NewPosition != null)
                    {
                        if (!_GlobalPosition)
                        {
                            typeObject.GameObject.transform.localPosition = new Vector3(_NewPosition.Value.x, _NewPosition.Value.y, _NewPosition.Value.z);
                        }
                        else
                        {
                            typeObject.GameObject.transform.position = new Vector3(_NewPosition.Value.x, _NewPosition.Value.y, _NewPosition.Value.z);
                        }
                    }
                    else
                    {
                        typeObject.GameObject.transform.localPosition = origin;
                    }

                    typeObject.GameObject.SetActive(true);

                    return typeObject;
                }

                // When the Queue is empty
                AdditionalObjectNeeded(this);
                // Creates a new GameObject
                AddObject();
            }
        }

        /// <summary>
        /// Enqueues the passed GameObject again
        /// </summary>
        /// <param name="_GameObject">GameObject to Enqueue</param>
        /// <param name="_PreviousParent">Should the Objects previous Parent (if there was one) should be set as its new Parent?</param>
        /// <param name="_NewPosition">Sets this GameObject to a new Position</param>
        public void ReturnObject(GameObject _GameObject, bool _PreviousParent = false, Vector3? _NewPosition = null)
        {
            // Searches for the passed GameObject in the list
            for (byte i = 0; i < objectsInUse.Count; i++)
            {
                if (_GameObject != objectsInUse[i].GameObject) continue;
                    // Removes it from the list and Enqueues it again
                    freeObjects.Enqueue(new ObjectAndType(_GameObject, objectsInUse[i].Component));
                    objectsInUse.RemoveAt(i);

                    _GameObject.SetActive(false);

                    if (_PreviousParent)
                    {
                        _GameObject.transform.SetParent(parent);
                    }
                    if (_NewPosition != null)
                    {
                        _GameObject.transform.localPosition = new Vector3(_NewPosition.Value.x, _NewPosition.Value.y, _NewPosition.Value.z);
                    }

                    break;
            }
        }

        /// <summary>
        /// Saves a reference to the GameObject and an additional Component
        /// </summary>
        public readonly struct ObjectAndType
        {
            #region Properties
                /// <summary>
                /// Reference to the GameObject
                /// </summary>
                public GameObject GameObject { get; }
                /// <summary>
                /// Reference to the Component
                /// </summary>
                public Component Component { get; }
            #endregion

            /// <param name="_GameObject">GameObject to save</param>
            /// <param name="_Component">Component to save</param>
            public ObjectAndType(GameObject _GameObject, Component _Component)
            {
                this.GameObject = _GameObject;
                this.Component = _Component;
            }
        }
    }
}