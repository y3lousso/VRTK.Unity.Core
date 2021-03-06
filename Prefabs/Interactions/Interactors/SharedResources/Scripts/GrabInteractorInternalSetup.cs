﻿namespace VRTK.Core.Prefabs.Interactions.Interactors
{
    using UnityEngine;
    using System.Collections.Generic;
    using VRTK.Core.Action;
    using VRTK.Core.Extension;
    using VRTK.Core.Data.Collection;
    using VRTK.Core.Tracking.Velocity;
    using VRTK.Core.Tracking.Collision;
    using VRTK.Core.Tracking.Collision.Active;
    using VRTK.Core.Prefabs.Interactions.Interactables;
    using VRTK.Core.Utility;

    /// <summary>
    /// Sets up the Interactor Prefab grab settings based on the provided user settings.
    /// </summary>
    public class GrabInteractorInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade.")]
        public InteractorFacade facade;
        #endregion

        #region Grab Settings
        /// <summary>
        /// The <see cref="BooleanAction"/> that will initiate the Interactor grab mechanism.
        /// </summary>
        [Header("Grab Settings"), Tooltip("The BooleanAction that will initiate the Interactor grab mechanism.")]
        public BooleanAction grabAction;
        /// <summary>
        /// The point in which to attach a grabbed Interactable to the Interactor.
        /// </summary>
        [Tooltip("The point in which to attach a grabbed Interactable to the Interactor.")]
        public GameObject attachPoint;
        /// <summary>
        /// The <see cref="VelocityTrackerProcessor"/> to measure the interactors current velocity for throwing on release.
        /// </summary>
        [Tooltip("The VelocityTrackerProcessor to measure the interactors current velocity for throwing on release.")]
        public VelocityTrackerProcessor velocityTracker;
        /// <summary>
        /// The <see cref="ActiveCollisionPublisher"/> for checking valid start grabbing action.
        /// </summary>
        [Tooltip("The ActiveCollisionPublisher for checking valid start touching collisions.")]
        public ActiveCollisionPublisher startGrabbingPublisher;
        /// <summary>
        /// The <see cref="ActiveCollisionPublisher"/> for checking valid stop grabbing action.
        /// </summary>
        [Tooltip("The ActiveCollisionPublisher for checking valid stop touching collisions.")]
        public ActiveCollisionPublisher stopGrabbingPublisher;
        /// <summary>
        /// The <see cref="GameObjectObservableSet"/> containing the currently grabbed objects.
        /// </summary>
        [Tooltip("The GameObjectSet containing the currently grabbed objects.")]
        public GameObjectObservableSet grabbedObjects;
        /// <summary>
        /// The <see cref="CountdownTimer"/> to determine grab precognition.
        /// </summary>
        [Tooltip("The CountdownTimer to determine grab precognition.")]
        public CountdownTimer precognitionTimer;
        /// <summary>
        /// The minimum timer value for the grab precognition <see cref="CountdownTimer"/>.
        /// </summary>
        [Tooltip("The minimum timer value for the grab precognition CountdownTimer.")]
        public float minPrecognitionTimer = 0.01f;
        #endregion

        /// <summary>
        /// A collection of currently grabbed GameObjects.
        /// </summary>
        public List<GameObject> GrabbedObjects => GetGrabbedObjects();

        /// <summary>
        /// Configures the action used to control grabbing.
        /// </summary>
        public virtual void ConfigureGrabAction()
        {
            if (grabAction != null && facade != null && facade.GrabAction != null)
            {
                grabAction.ClearSources();
                grabAction.AddSource(facade.GrabAction);
            }
        }

        /// <summary>
        /// Configures the velocity tracker used for grabbing.
        /// </summary>
        public virtual void ConfigureVelocityTrackers()
        {
            if (velocityTracker != null && facade != null && facade.VelocityTracker != null)
            {
                velocityTracker.velocityTrackers.Clear();
                velocityTracker.velocityTrackers.Add(facade.VelocityTracker);
            }
        }

        /// <summary>
        /// Configures the <see cref="ActiveCollisionPublisher"/> components for touching and untouching.
        /// </summary>
        public virtual void ConfigurePublishers()
        {
            if (startGrabbingPublisher != null)
            {
                startGrabbingPublisher.payload.sourceContainer = attachPoint;
            }

            if (stopGrabbingPublisher != null)
            {
                stopGrabbingPublisher.payload.sourceContainer = attachPoint;
            }
        }

        /// <summary>
        /// Configures the <see cref="CountdownTimer"/> components for grab precognition.
        /// </summary>
        public virtual void ConfigureGrabPrecognition()
        {
            if (facade.GrabPrecognition < minPrecognitionTimer)
            {
                facade.GrabPrecognition = minPrecognitionTimer;
            }
            precognitionTimer.startTime = facade.GrabPrecognition;
        }

        /// <summary>
        /// Attempt to grab an Interactable to the current Interactor utilising custom collision data.
        /// </summary>
        /// <param name="interactable">The Interactable to attempt to grab.</param>
        /// <param name="collision">Custom collision data.</param>
        /// <param name="collider">Custom collider data.</param>
        public virtual void Grab(InteractableFacade interactable, Collision collision, Collider collider)
        {
            if (interactable == null)
            {
                return;
            }

            Ungrab();
            startGrabbingPublisher.SetActiveCollisions(CreateActiveCollisionsEventData(interactable.gameObject, collision, collider));
            ProcessGrabAction(startGrabbingPublisher, true);
            if (interactable.grabType == InteractableFacade.ActiveType.Toggle)
            {
                ProcessGrabAction(startGrabbingPublisher, false);
            }
        }

        /// <summary>
        /// Attempt to ungrab currently grabbed Interactables to the current Interactor.
        /// </summary>
        public virtual void Ungrab()
        {
            if (GrabbedObjects.Count == 0)
            {
                return;
            }

            InteractableFacade interactable = GrabbedObjects[0].TryGetComponent<InteractableFacade>(true, true);
            if (interactable.grabType == InteractableFacade.ActiveType.Toggle)
            {
                ProcessGrabAction(startGrabbingPublisher, true);
            }
            ProcessGrabAction(stopGrabbingPublisher, false);
        }

        protected virtual void ProcessGrabAction(ActiveCollisionPublisher publisher, bool actionState)
        {
            precognitionTimer.enabled = false;
            if (grabAction.Value != actionState)
            {
                grabAction.Receive(actionState);
            }
            if (grabAction.Value)
            {
                publisher.Publish();
            }
            precognitionTimer.enabled = true;
        }

        protected virtual void OnEnable()
        {
            ConfigureGrabAction();
            ConfigureVelocityTrackers();
            ConfigurePublishers();
            ConfigureGrabPrecognition();
        }

        /// <summary>
        /// Retreives a collection of currently grabbed GameObjects.
        /// </summary>
        /// <returns>The currently grabbed GameObjects.</returns>
        protected virtual List<GameObject> GetGrabbedObjects()
        {
            List<GameObject> returnList = new List<GameObject>();

            if (grabbedObjects == null)
            {
                return returnList;
            }

            returnList.AddRange(grabbedObjects.Elements);
            return returnList;
        }

        protected virtual ActiveCollisionsContainer.EventData CreateActiveCollisionsEventData(GameObject forwardSource, Collision collision = null, Collider collider = null)
        {
            collider = (collider == null ? forwardSource.GetComponentInChildren<Collider>() : collider);
            CollisionNotifier.EventData collisionPayload = new CollisionNotifier.EventData();
            collisionPayload.Set(forwardSource.TryGetComponent<Component>(), collider.isTrigger, collision, collider);
            ActiveCollisionsContainer.EventData activeCollisionPayload = new ActiveCollisionsContainer.EventData();
            List<CollisionNotifier.EventData> collisionList = new List<CollisionNotifier.EventData>();
            collisionList.Add(collisionPayload);
            activeCollisionPayload.Set(collisionList);
            return activeCollisionPayload;
        }
    }
}