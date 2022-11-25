using System.Collections.Generic;
using System.Linq;
using RadialUI;
using Xunit;

namespace RadialUnitTests
{

    // First simple test and asserts regardless of inputs, return true. Used for buttons on minis as default
    public class RadialUIPluginTests
    {
        public static bool CustomShouldShowMenu(string menuText, string miniId, string targetId) => true;

        [Theory]
        [InlineData("","","")]
        [InlineData(null,null,null)]
        public void TestAlwaysTrue(string s1, string s2, string s3)
        {
            Assert.True(RadialUIPlugin.AlwaysTrue(s1, s2, s3));
        }

        [Fact]
        public void TestAddRemoveOnAddNewKeyIfDoesNotExist()
        {
            Dictionary<string, List<RadialCheckRemove>> data = new Dictionary<string, List<RadialCheckRemove>>();
            string key = "my.Plugin.Key";
            string value = "my.text.value";
            
            Assert.Empty(data);
            Assert.DoesNotContain(key,data.Keys);

            RadialUIPlugin.AddRemoveOn(data,key,value,CustomShouldShowMenu);

            Assert.Contains(key, data.Keys);
            Assert.True(data[key].SingleOrDefault( dat => dat.TitleToRemove == value) != null);
            Assert.True(data[key].SingleOrDefault(dat => dat.TitleToRemove == value).ShouldRemoveCallback == CustomShouldShowMenu);
        }

        [Fact]
        public void TestAddRemoveOnNull()
        {
            Dictionary<string, List<RadialCheckRemove>> data = new Dictionary<string, List<RadialCheckRemove>>();
            string key = "my.Plugin.Key";
            string value = "my.text.value";

            Assert.Empty(data);
            Assert.DoesNotContain(key, data.Keys);

            RadialUIPlugin.AddRemoveOn(data, key, value, null);

            Assert.Contains(key, data.Keys);
            Assert.True(data[key].SingleOrDefault(dat => dat.TitleToRemove == value) != null);
            Assert.True(data[key].SingleOrDefault(dat => dat.TitleToRemove == value).ShouldRemoveCallback == RadialUIPlugin.AlwaysTrue);
        }

        [Fact]
        public void TestRemoveRemoveOnPass()
        {
            Dictionary<string, List<RadialCheckRemove>> data = new Dictionary<string, List<RadialCheckRemove>>();
            string key = "my.Plugin.Key";
            string value = "my.text.value";

            RadialUIPlugin.AddRemoveOn(data, key, value, null);

            Assert.Contains(key, data.Keys);
            Assert.True(data[key].SingleOrDefault(dat => dat.TitleToRemove == value) != null);

            var result = RadialUIPlugin.RemoveRemoveOn(data, key, value);
            Assert.True(result);
        }

        [Fact]
        public void TestRemoveRemoveOnFailWithWrongKey()
        {
            Dictionary<string, List<RadialCheckRemove>> data = new Dictionary<string, List<RadialCheckRemove>>();
            string key = "my.Plugin.Key";
            string value = "my.text.value";

            RadialUIPlugin.AddRemoveOn(data, key, value, null);

            Assert.Contains(key, data.Keys);
            Assert.True(data[key].SingleOrDefault(dat => dat.TitleToRemove == value) != null);

            var result = RadialUIPlugin.RemoveRemoveOn(data, "wrong key", "Doesn't exist");
            Assert.False(result);
        }

        [Fact]
        public void TestRemoveRemoveOnFailWithWrongValue()
        {
            Dictionary<string, List<RadialCheckRemove>> data = new Dictionary<string, List<RadialCheckRemove>>();
            string key = "my.Plugin.Key";
            string value = "my.text.value";

            RadialUIPlugin.AddRemoveOn(data, key, value, null);

            Assert.Contains(key, data.Keys);
            Assert.True(data[key].SingleOrDefault(dat => dat.TitleToRemove == value) != null);

            var result = RadialUIPlugin.RemoveRemoveOn(data, key, "Doesn't exist");
            Assert.False(result);
        }
    }
}
