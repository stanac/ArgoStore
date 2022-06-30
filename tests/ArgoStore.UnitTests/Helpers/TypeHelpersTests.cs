using ArgoStore.Helpers;

namespace ArgoStore.UnitTests.Helpers
{
    public class TypeHelpersTests
    {
        [Fact]
        public void IsCollectionType_ArrayOfInt_ReturnsTrue()
        {
            bool isCollectionType = TypeHelpers.IsCollectionType(typeof(int[]));
            isCollectionType.Should().BeTrue();
        }

        [Fact]
        public void IsCollectionType_ListOfInt_ReturnsTrue()
        {
            bool isCollectionType = TypeHelpers.IsCollectionType(typeof(List<int>));
            isCollectionType.Should().BeTrue();
        }

        [Fact]
        public void IsCollectionType_IListOfInt_ReturnsTrue()
        {
            bool isCollectionType = TypeHelpers.IsCollectionType(typeof(IList<int>));
            isCollectionType.Should().BeTrue();
        }

        [Fact]
        public void IsCollectionType_IEnumerableOfInt_ReturnsTrue()
        {
            bool isCollectionType = TypeHelpers.IsCollectionType(typeof(IEnumerable<int>));
            isCollectionType.Should().BeTrue();
        }

        [Fact]
        public void IsCollectionType_Int_ReturnsTrue()
        {
            bool isCollectionType = TypeHelpers.IsCollectionType(typeof(int));
            isCollectionType.Should().BeFalse();
        }

        [Fact]
        public void IsCollectionType_NullableInt_ReturnsTrue()
        {
            bool isCollectionType = TypeHelpers.IsCollectionType(typeof(int?));
            isCollectionType.Should().BeFalse();
        }

        [Fact]
        public void GetCollectionElementType_TypeIsArray_ReturnsCorrectType()
        {
            Type type = TypeHelpers.GetCollectionElementType(typeof(int[]));
            type.Should().Be(typeof(int));
        }

        [Fact]
        public void GetCollectionElementType_TypeIsListOfT_ReturnsCorrectType()
        {
            Type type = TypeHelpers.GetCollectionElementType(typeof(List<int>));
            type.Should().Be(typeof(int));
        }

        [Fact]
        public void GetCollectionElementType_TypeIsIListOfT_ReturnsCorrectType()
        {
            Type type = TypeHelpers.GetCollectionElementType(typeof(IList<int>));
            type.Should().Be(typeof(int));
        }

        [Fact]
        public void GetCollectionElementType_TypeIsIEnumerableOfT_ReturnsCorrectType()
        {
            Type type = TypeHelpers.GetCollectionElementType(typeof(IEnumerable<int>));
            type.Should().Be(typeof(int));
        }

        [Fact]
        public void GetCollectionElementType_TypeIsNullableOfT_ThrowsException()
        {
            Action a = () => TypeHelpers.GetCollectionElementType(typeof(int?));
            a.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void CreateIEnumerableOfType_CreatesCorrectType()
        {
            Type type = TypeHelpers.CreateIEnumerableOfType(typeof(int?));
            type.Should().Be(typeof(IEnumerable<int?>));
        }
    }
}
