using NUnit.Framework;

// Tests run sequentially
// True isolation is impossible since we have only one shared between tests account, so each test instead resets
// the account to a clean state in [SetUp]. In a real environment i would parallelize by giving each test its own account 
[assembly: NonParallelizable]
