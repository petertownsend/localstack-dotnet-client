﻿namespace LocalStack.Client.Functional.Tests.Scenarios.SQS;

[Collection(nameof(LocalStackLegacyCollection))]
public class SqsLegacyTests : SqsScenario
{
    public SqsLegacyTests(TestFixture testFixture)
        : base(testFixture, TestConstants.LegacyLocalStackConfig)
    {
    }
}
