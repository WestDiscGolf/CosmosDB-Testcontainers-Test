using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace Api.Tests.Infrastructure;

/// <summary>
/// Extension attribute of the <see cref="InlineAutoDataAttribute"/> to allow for Moq based items as well.
/// </summary>
public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InlineAutoMoqDataAttribute"/> class.
    /// </summary>
    /// <param name="values">The data values required for the tests.</param>
    public InlineAutoMoqDataAttribute(params object[] values)
        : base(new AutoMoqDataAttribute(), values)
    {
    }

    /// <summary>
    /// Private attribute to hook up the Moq functionality into the pipeline of the <see cref="IFixture"/> functionality.
    /// </summary>
    private class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}