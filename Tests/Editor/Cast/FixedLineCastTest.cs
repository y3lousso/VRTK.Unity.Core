﻿using VRTK.Core.Cast;

namespace Test.VRTK.Core.Cast
{
    using UnityEngine;
    using NUnit.Framework;
    using Test.VRTK.Core.Utility.Mock;

    public class FixedLineCastTest
    {
        private GameObject containingObject;
        private FixedLineCastMock subject;

        [SetUp]
        public void SetUp()
        {
            containingObject = new GameObject();
            subject = containingObject.AddComponent<FixedLineCastMock>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(subject);
            Object.DestroyImmediate(containingObject);
        }

        [Test]
        public void CastPoints()
        {
            UnityEventListenerMock castResultsChangedMock = new UnityEventListenerMock();
            subject.ResultsChanged.AddListener(castResultsChangedMock.Listen);
            subject.origin = subject.gameObject;
            subject.currentLength = 10f;

            subject.ManualOnEnable();
            subject.Process();

            Vector3 expectedStart = Vector3.zero;
            Vector3 expectedEnd = new Vector3(0f, 0f, 10f);

            Assert.AreEqual(expectedStart, subject.Points[0]);
            Assert.AreEqual(expectedEnd, subject.Points[1]);
            Assert.IsTrue(castResultsChangedMock.Received);
        }
    }

    public class FixedLineCastMock : FixedLineCast
    {
        public void ManualOnEnable()
        {
            OnEnable();
        }

        public void ManualOnDisable()
        {
            OnDisable();
        }
    }
}