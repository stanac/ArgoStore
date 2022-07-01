using ArgoStore.Helpers;
using TypeExtensions = ArgoStore.Helpers.TypeExtensions;

namespace ArgoStore.UnitTests.Helpers;

public class TypeHelpersTests
{
    [Fact]
    public void IsCollectionType_ArrayOfInt_ReturnsTrue()
    {
        bool isCollectionType = typeof(int[]).IsCollectionType();
        isCollectionType.Should().BeTrue();
    }

    [Fact]
    public void IsCollectionType_ListOfInt_ReturnsTrue()
    {
        bool isCollectionType = typeof(List<int>).IsCollectionType();
        isCollectionType.Should().BeTrue();
    }

    [Fact]
    public void IsCollectionType_IListOfInt_ReturnsTrue()
    {
        bool isCollectionType = typeof(IList<int>).IsCollectionType();
        isCollectionType.Should().BeTrue();
    }

    [Fact]
    public void IsCollectionType_IEnumerableOfInt_ReturnsTrue()
    {
        bool isCollectionType = typeof(IEnumerable<int>).IsCollectionType();
        isCollectionType.Should().BeTrue();
    }

    [Fact]
    public void IsCollectionType_Int_ReturnsTrue()
    {
        bool isCollectionType = typeof(int).IsCollectionType();
        isCollectionType.Should().BeFalse();
    }

    [Fact]
    public void IsCollectionType_NullableInt_ReturnsTrue()
    {
        bool isCollectionType = typeof(int?).IsCollectionType();
        isCollectionType.Should().BeFalse();
    }

    [Fact]
    public void GetCollectionElementType_TypeIsArray_ReturnsCorrectType()
    {
        Type type = typeof(int[]).GetCollectionElementType();
        type.Should().Be(typeof(int));
    }

    [Fact]
    public void GetCollectionElementType_TypeIsListOfT_ReturnsCorrectType()
    {
        Type type = typeof(List<int>).GetCollectionElementType();
        type.Should().Be(typeof(int));
    }

    [Fact]
    public void GetCollectionElementType_TypeIsIListOfT_ReturnsCorrectType()
    {
        Type type = typeof(IList<int>).GetCollectionElementType();
        type.Should().Be(typeof(int));
    }

    [Fact]
    public void GetCollectionElementType_TypeIsIEnumerableOfT_ReturnsCorrectType()
    {
        Type type = typeof(IEnumerable<int>).GetCollectionElementType();
        type.Should().Be(typeof(int));
    }

    [Fact]
    public void GetCollectionElementType_TypeIsNullableOfT_ThrowsException()
    {
        Action a = () => typeof(int?).GetCollectionElementType();
        a.Should().Throw<ArgumentException>();
    }
        
    [Fact]
    public void CreateIEnumerableOfType_CreatesCorrectType()
    {
        Type type = typeof(int?).CreateIEnumerableOfType();
        type.Should().Be(typeof(IEnumerable<int?>));
    }
}