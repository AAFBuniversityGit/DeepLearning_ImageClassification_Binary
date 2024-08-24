# DeepLearning_ImageClassification_Binary

This repository contains a project for binary image classification using ML.NET. The model is designed to detect bridge deck damage by classifying images into two categories. The solution utilizes transfer learning with a pretrained ResNet model, which allows efficient training and high accuracy with limited labeled data.

## Features

- **Transfer Learning**: Leverages a pretrained ResNet model for efficient training.
- **Binary Classification**: Classifies images into two categories.
- **Deep Learning**: Uses neural networks to achieve accurate predictions.
- **ML.NET**: The entire pipeline is built using ML.NET.

## How It Works

The model follows these key steps:

1. **Data Loading**: Images are loaded from the specified directory and labeled based on the folder name.
2. **Preprocessing**: Images are transformed into a format suitable for the model, and labels are converted into key values.
3. **Training**: The model is trained using transfer learning, where the pretrained ResNet model is fine-tuned on the provided data.
4. **Evaluation**: The model is evaluated on a validation set to assess its accuracy.
5. **Prediction**: The trained model is used to classify new images and output predictions.

## Requirements

- .NET 6.0 SDK or later
- ML.NET 2.0 or later

## How to Run

1. Clone the repository.
2. Ensure the dataset is placed in the `Assets` directory.
3. Run the project using your preferred IDE or the .NET CLI.

```bash
git clone https://github.com/AAFBuniversityGit/DeepLearning_ImageClassification_Binary.git
cd DeepLearning_ImageClassification_Binary
dotnet run
