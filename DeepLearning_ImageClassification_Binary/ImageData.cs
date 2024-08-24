namespace DeepLearning_ImageClassification_Binary;

internal class ImageData
{
    public string? ImagePath { get; set; }
    public string? Label { get; set; }
}

internal class ModelInput
{
    public byte[]? Image { get; set; }
    public uint LabelAsKey { get; set; }
    public string? ImagePath { get; set; }
    public string? Label { get; set; }
}

internal class ModelOutput
{
    public string? ImagePath { get; set; }
    public string? Label { get; set; }
    public string? PredictedLabel { get; set; }
}