﻿using VRTK.Core.Tracking.Modification;

namespace Test.VRTK.Core.Tracking.Modification
{
    using UnityEngine;
    using NUnit.Framework;

    public class ComponentEnabledStateModifierTest
    {
        private GameObject containingObject;
        private ComponentEnabledStateModifier subject;

        [SetUp]
        public void SetUp()
        {
            containingObject = new GameObject();
            subject = containingObject.AddComponent<ComponentEnabledStateModifier>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(containingObject);
        }

        [Test]
        public void SetEnabledStateOfBehaviour()
        {
            Behaviour behaviour = containingObject.AddComponent<Light>();
            subject.types.Add(typeof(Light));
            subject.target = containingObject;

            Assert.IsTrue(behaviour.enabled);
            subject.SetEnabledState(false);
            Assert.IsFalse(behaviour.enabled);
            subject.SetEnabledState(true);
            Assert.IsTrue(behaviour.enabled);
        }

        [Test]
        public void SetEnabledStateOfRenderer()
        {
            MeshRenderer renderer = containingObject.AddComponent<MeshRenderer>();
            subject.types.Add(typeof(MeshRenderer));
            subject.target = containingObject;

            Assert.IsTrue(renderer.enabled);
            subject.SetEnabledState(false);
            Assert.IsFalse(renderer.enabled);
            subject.SetEnabledState(true);
            Assert.IsTrue(renderer.enabled);
        }

        [Test]
        public void SetEnabledStateOfBehaviourAndRenderer()
        {
            Behaviour behaviour = containingObject.AddComponent<Light>();
            MeshRenderer renderer = containingObject.AddComponent<MeshRenderer>();
            subject.types.Add(typeof(Light));
            subject.types.Add(typeof(MeshRenderer));
            subject.target = containingObject;

            Assert.IsTrue(behaviour.enabled);
            Assert.IsTrue(renderer.enabled);
            subject.SetEnabledState(false);
            Assert.IsFalse(behaviour.enabled);
            Assert.IsFalse(renderer.enabled);
            subject.SetEnabledState(true);
            Assert.IsTrue(behaviour.enabled);
            Assert.IsTrue(renderer.enabled);
        }

        [Test]
        public void SetEnabledStateOfInvalidType()
        {
            Behaviour behaviour = containingObject.AddComponent<Light>();
            subject.types.Add(typeof(Renderer));
            subject.target = containingObject;

            Assert.IsTrue(behaviour.enabled);
            subject.SetEnabledState(false);
            Assert.IsTrue(behaviour.enabled);
        }

        [Test]
        public void SetEnabledStateInvalidTarget()
        {
            Behaviour behaviour = containingObject.AddComponent<Light>();
            subject.types.Add(typeof(Light));

            Assert.IsTrue(behaviour.enabled);
            subject.SetEnabledState(false);
            Assert.IsTrue(behaviour.enabled);
        }

        [Test]
        public void SetEnabledStateInactiveGameObject()
        {
            Behaviour behaviour = containingObject.AddComponent<Light>();
            subject.types.Add(typeof(Light));
            subject.target = containingObject;

            subject.gameObject.SetActive(false);

            Assert.IsTrue(behaviour.enabled);
            subject.SetEnabledState(false);
            Assert.IsTrue(behaviour.enabled);
        }


        [Test]
        public void SetEnabledStateInactiveComponent()
        {
            Behaviour behaviour = containingObject.AddComponent<Light>();
            subject.types.Add(typeof(Light));
            subject.target = containingObject;

            subject.enabled = false;

            Assert.IsTrue(behaviour.enabled);
            subject.SetEnabledState(false);
            Assert.IsTrue(behaviour.enabled);
        }
    }
}