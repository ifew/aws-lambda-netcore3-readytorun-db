# Example AWS Lambda C# .NET Core 3.0 Release though Custom Runtime (Amazon.Lambda.RuntimeSupport) and ReadyToRun with connect MySQL via not using any ORM

docker run --rm -v "$PWD":/var/task lambci/lambda:dotnetcore2.1 aws-lambda-netcore3-readytorun::aws_lambda_netcore3_readytorun.Function::FunctionHandler ''