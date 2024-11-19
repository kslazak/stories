namespace StoriesUnitTests;

public static class TestUtils
{
    public static List<int> CreateListOfIds(int count)
    {
        return Enumerable.Range(0, count).ToList();
    }
}