﻿using DeepLearning_ImageClassification_Binary;
using Microsoft.ML;
using Microsoft.ML.Vision;

var projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
var assetsRelativePath = Path.Combine(projectDirectory, "Assets");

MLContext mlContext = new();

IEnumerable<ImageData> images = LoadImagesFromDirectory(folder: assetsRelativePath, useFolderNameAsLabel: true);

IDataView imageData = mlContext.Data.LoadFromEnumerable(images);

IDataView shuffledData = mlContext.Data.ShuffleRows(imageData);

var preprocessingPipeline = mlContext.Transforms.Conversion.MapValueToKey(
        inputColumnName: "Label",
        outputColumnName: "LabelAsKey")
    .Append(mlContext.Transforms.LoadRawImageBytes(
        outputColumnName: "Image",
        imageFolder: assetsRelativePath,
        inputColumnName: "ImagePath"));

IDataView preProcessedData = preprocessingPipeline
    .Fit(shuffledData)
    .Transform(shuffledData);

DataOperationsCatalog.TrainTestData trainSplit = mlContext.Data.TrainTestSplit(data: preProcessedData, testFraction: 0.3);
DataOperationsCatalog.TrainTestData validationTestSplit = mlContext.Data.TrainTestSplit(trainSplit.TestSet);

IDataView trainSet = trainSplit.TrainSet;
IDataView validationSet = validationTestSplit.TrainSet;
IDataView testSet = validationTestSplit.TestSet;

var classifierOptions = new ImageClassificationTrainer.Options()
{
    FeatureColumnName = "Image",
    LabelColumnName = "LabelAsKey",
    ValidationSet = validationSet,
    Arch = ImageClassificationTrainer.Architecture.ResnetV2101,
    MetricsCallback = (metrics) => Console.WriteLine(metrics),
    TestOnTrainSet = false,
    ReuseTrainSetBottleneckCachedValues = true,
    ReuseValidationSetBottleneckCachedValues = true
};

var trainingPipeline = mlContext.MulticlassClassification.Trainers.ImageClassification(classifierOptions)
    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

ITransformer trainedModel = trainingPipeline.Fit(trainSet);

ClassifySingleImage(mlContext, testSet, trainedModel);

ClassifyImages(mlContext, testSet, trainedModel);





static IEnumerable<ImageData> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel = true)
{
    var files = Directory.GetFiles(folder, "*",
        searchOption: SearchOption.AllDirectories);

    foreach (var file in files)
    {
        if ((Path.GetExtension(file) != ".jpg") && (Path.GetExtension(file) != ".png"))
            continue;

        var label = Path.GetFileName(file);

        if (useFolderNameAsLabel)
            label = Directory.GetParent(file)?.Name;
        else
        {
            for (int index = 0; index < label.Length; index++)
            {
                if (!char.IsLetter(label[index]))
                {
                    label = label[..index];
                    break;
                }
            }
        }

        yield return new ImageData()
        {
            ImagePath = file,
            Label = label
        };
    }
}

static void OutputPrediction(ModelOutput prediction)
{
    string? imageName = Path.GetFileName(prediction.ImagePath);
    Console.WriteLine($"Image: {imageName} | Actual Value: {prediction.Label} | Predicted Value: {prediction.PredictedLabel}");
}

static void ClassifySingleImage(MLContext mlContext, IDataView data, ITransformer trainedModel)
{
    PredictionEngine<ModelInput, ModelOutput> predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);

    ModelInput image = mlContext.Data.CreateEnumerable<ModelInput>(data, reuseRowObject: true).First();

    ModelOutput prediction = predictionEngine.Predict(image);

    Console.WriteLine("Classifying single image");
    OutputPrediction(prediction);
}

static void ClassifyImages(MLContext mlContext, IDataView data, ITransformer trainedModel)
{
    IDataView predictionData = trainedModel.Transform(data);

    IEnumerable<ModelOutput> predictions = mlContext.Data.CreateEnumerable<ModelOutput>(predictionData, reuseRowObject: true).Take(10);

    Console.WriteLine("Classifying multiple images");
    foreach (var prediction in predictions)
    {
        OutputPrediction(prediction);
    }
}