using System.Text.Json;

namespace SFA.DAS.ApiContracts.Build.Tests.ApiResponses;

public class WhenSerialisingResponseDtosWithSystemTextJson
{
    [Test]
    public void Then_Nullable_Enum_Property_Deserialises_From_String_Value()
    {
        var json = """{"status":"Name"}""";

        var result = JsonSerializer.Deserialize<PostDasRequest>(json);

        result!.Status.Should().Be(DasRequestSortOrder.Name);
    }

    [Test]
    public void Then_Nullable_Enum_Property_Deserialises_As_Null_When_Json_Value_Is_Null()
    {
        var json = """{"status":null}""";

        var result = JsonSerializer.Deserialize<PostDasRequest>(json);

        result!.Status.Should().BeNull();
    }

    [Test]
    public void Then_Nullable_Enum_Property_Is_Null_When_Omitted_From_Json()
    {
        var json = """{}""";

        var result = JsonSerializer.Deserialize<PostDasRequest>(json);

        result!.Status.Should().BeNull();
    }

    [Test]
    public void Then_Nullable_Enum_Property_Serialises_As_String_When_Set()
    {
        var dto = new PostDasRequest { Status = DasRequestSortOrder.Created };

        var json = JsonSerializer.Serialize(dto);

        json.Should().Contain("\"status\":\"Created\"");
    }

    [Test]
    public void Then_Nullable_Enum_Property_Is_Omitted_From_Json_When_Null()
    {
        var dto = new PostDasRequest { Status = null };

        var json = JsonSerializer.Serialize(dto);

        json.Should().NotContain("\"status\"");
    }

    [Test]
    public void Then_NonNullable_Bool_Property_Serialises_Without_Throwing()
    {
        // WhenWritingNull is invalid on value-type members — the serialiser should not throw.
        var dto = new PatchDasItemResponse { Id = Guid.NewGuid(), Updated = true };

        var act = () => JsonSerializer.Serialize(dto);

        act.Should().NotThrow();
    }

    [Test]
    public void Then_NonNullable_Bool_Property_Serialises_With_Value()
    {
        var dto = new PatchDasItemResponse { Id = Guid.NewGuid(), Updated = true };

        var json = JsonSerializer.Serialize(dto);

        json.Should().Contain("\"updated\":true");
    }
}
