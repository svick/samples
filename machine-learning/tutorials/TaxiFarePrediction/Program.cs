﻿// <Snippet1>
using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Transforms;
using Microsoft.ML.Transforms.Categorical;
using Microsoft.ML.Transforms.Normalizers;
using Microsoft.ML.Transforms.Text;
// </Snippet1>

namespace TaxiFarePrediction
{
    class Program
    {
        // <Snippet2>
        static readonly string _trainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-train.csv");
        static readonly string _testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-test.csv");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "Model.zip");
        static TextLoader _textLoader;
        // </Snippet2>

        static void Main(string[] args)
        {
            Console.WriteLine(Environment.CurrentDirectory);

            // <Snippet3>
            MLContext mlContext = new MLContext(seed: 0);
            // </Snippet3>

            // <Snippet4>
            _textLoader = mlContext.Data.TextReader(new TextLoader.Arguments()
            {
                Separator = ",",
                HasHeader = true,
                Column = new[]
                            {
                                new TextLoader.Column("VendorId", DataKind.Text, 0),
                                new TextLoader.Column("RateCode", DataKind.Text, 1),
                                new TextLoader.Column("PassengerCount", DataKind.R4, 2),
                                new TextLoader.Column("TripTime", DataKind.R4, 3),
                                new TextLoader.Column("TripDistance", DataKind.R4, 4),
                                new TextLoader.Column("PaymentType", DataKind.Text, 5),
                                new TextLoader.Column("FareAmount", DataKind.R4, 6)
                            }
            }
            );
            // </Snippet4>

            // <Snippet5>
            var model = Train(mlContext, _trainDataPath);
            // </Snippet5>

            // <Snippet14>
            Evaluate(mlContext, model);
            // </Snippet14>

            // <Snippet20>
            TestSinglePrediction(mlContext);
            // </Snippet20>
        }
        
        public static ITransformer Train(MLContext mlContext, string dataPath)
        {
            // <Snippet6>
            IDataView dataView = _textLoader.Read(dataPath);
            // </Snippet6>

            // <Snippet7>
            var pipeline = mlContext.Transforms.CopyColumns("FareAmount", "Label")
            // </Snippet7>
                    // <Snippet8>
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding("VendorId"))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding("RateCode"))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding("PaymentType"))
                    // </Snippet8>
                    // <Snippet9>
                    .Append(mlContext.Transforms.Concatenate("Features", "VendorId", "RateCode", "PassengerCount", "TripTime", "TripDistance", "PaymentType"))
                    // </Snippet9>
                    // <Snippet10>
                    .Append(mlContext.Regression.Trainers.FastTree());
                    // </Snippet10>


            Console.WriteLine("=============== Create and Train the Model ===============");
            
            // <Snippet11>
            var model = pipeline.Fit(dataView);
            // </Snippet11>

            Console.WriteLine("=============== End of training ===============");
            Console.WriteLine();
            // <Snippet12>
            SaveModelAsFile(mlContext, model);
            return model;
            // </Snippet12>
        }

        private static void Evaluate(MLContext mlContext, ITransformer model)
        {
            // <Snippet15>
            IDataView dataView = _textLoader.Read(_testDataPath);
            // </Snippet15>

            // <Snippet16>
            var predictions = model.Transform(dataView);
            // </Snippet16>
            // <Snippet17>
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");
            // </Snippet17>

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            // <Snippet18>
            Console.WriteLine($"*       R2 Score:      {metrics.RSquared:0.##}");
            // </Snippet18>
            // <Snippet19>
            Console.WriteLine($"*       RMS loss:      {metrics.Rms:#.##}");
            // </Snippet19>
            Console.WriteLine($"*************************************************");

        }

        private static void TestSinglePrediction(MLContext mlContext)
        {
            //load the model
            // <Snippet21>
            ITransformer loadedModel;
            using (var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                loadedModel = mlContext.Model.Load(stream);
            }
            // </Snippet21>

            //Prediction test
            // Create prediction function and make prediction.
            // <Snippet22>
            var predictionFunction = loadedModel.MakePredictionFunction<TaxiTrip, TaxiTripFarePrediction>(mlContext);
            // </Snippet22>
            //Sample: 
            //vendor_id,rate_code,passenger_count,trip_time_in_secs,trip_distance,payment_type,fare_amount
            //VTS,1,1,1140,3.75,CRD,15.5
            // <Snippet23>
            var taxiTripSample = new TaxiTrip()
            {
                VendorId = "VTS",
                RateCode = "1",
                PassengerCount = 1,
                TripTime = 1140,
                TripDistance = 3.75f,
                PaymentType = "CRD",
                FareAmount = 0 // To predict. Actual/Observed = 15.5
            };
            // </Snippet23>
            // <Snippet24>
            var prediction = predictionFunction.Predict(taxiTripSample);
            // </Snippet24>
            // <Snippet25>
            Console.WriteLine($"**********************************************************************");
            Console.WriteLine($"Predicted fare: {prediction.FareAmount:0.####}, actual fare: 15.5");
            Console.WriteLine($"**********************************************************************");
            // </Snippet25>
        }

        private static void SaveModelAsFile(MLContext mlContext, ITransformer model)
        {
            // <Snippet13> 
            using (var fileStream = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                mlContext.Model.Save(model, fileStream);
            // </Snippet13>

            Console.WriteLine("The model is saved to {0}", _modelPath);
        }
    }
}
