// #define logStart             
// #define logCancellation
// #define logEnd

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ganymed.Utils.Callbacks;
using Ganymed.Utils.ExtensionMethods;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = UnityEditor.Compilation.Assembly;

namespace Ganymed.Utils.Editor.AttributeValidation
{
    /// <summary>
    /// Class responsible for validation of custom attributes.
    /// </summary>
    internal static partial class AttributeReflection
    {
        #region --- [FIELDS] ---

        private const BindingFlags MemberFlags // MemberFlags contains a definition for every possible member.
            = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        // --- Cancellation / Task resources. 
        private static volatile CancellationTokenSource source = new CancellationTokenSource();
        private static CancellationToken ct;

        private static Assembly[] AssemblyDefinitions;
        
        private static volatile float time;
        
        #endregion
        
        /// <summary>
        /// Initialize will start a task that will begin reflection on every assembly and continue with individual
        /// validation of custom attributes. 
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            time = Time.realtimeSinceStartup;
            
            #if logStart
            Debug.Log($"started attribute reflection task");
            #endif
            
            AssemblyDefinitions = CompilationPipeline.GetAssemblies();
            
            Task.Run(delegate
            {
                ct.ThrowIfCancellationRequested();
                try{
                    AppDomain.CurrentDomain.GetTypesFromUnityAssemblies(
                        AssemblyDefinitions,
                        out var types,
                        out var fieldInfos,
                        out var propertyInfos,
                        out var methodInfos,
                        out var constructorInfos,
                        out var eventInfos,
                        out var memberInfos);

                    ReflectAttributeTarget(types);
                    ReflectValidBindingFlagsAttribute(memberInfos);
                    ReflectTargetParameterTypeRestrictions(methodInfos);
                    ReflectTargetTypeRestriction(propertyInfos, fieldInfos);
                    ReflectPropertyAccessRequirement(propertyInfos);
                    ReflectRequiredAttributes(types, fieldInfos, propertyInfos, methodInfos, constructorInfos, eventInfos);
                }
                catch (Exception exception)
                {
                    // --- Log the exception if it was not thrown by the task cancellation. 
                    if (!(exception is ThreadAbortException)) {
                        Debug.LogException(exception);
                    }
#if logCancellation
                Debug.Log($"cancelled attribute reflection task");
#endif
                }
                finally
                {
                    ResetReflectionTaskResources();
                }
            }, ct).Then(delegate
            {
                #if logEnd
                Debug.Log($"successfully ended attribute reflection task after: {Time.realtimeSinceStartup - time:0.00}s");
                #endif
            });
        }

        /// <summary>
        /// Method cancels ongoing reflection tasks if present and resets the CancellationTokenSource and
        /// CancellationToken. 
        /// </summary>
        private static void ResetReflectionTaskResources()
        {
            source.Cancel();
            source.Dispose();
            source = new CancellationTokenSource();
            ct = source.Token;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        static AttributeReflection()
        {
            // --- Subscribing to certain events that will cancel the reflection task prematurely.
            UnityEventCallbacks.AddEventListener(
                listener:ResetReflectionTaskResources, 
                removePreviousListener: true, 
                ApplicationState.EditAndPlayMode,
                UnityEventType.ApplicationQuit,
                UnityEventType.TransitionEditPlayMode,
                UnityEventType.PreProcessorBuild);
        }
    }
}