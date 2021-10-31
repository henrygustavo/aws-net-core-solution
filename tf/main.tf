provider "aws" {
  region                      = "us-west-1"
  access_key                  = "anaccesskey"
  secret_key                  = "anaccesskey"
  skip_credentials_validation = true
  skip_metadata_api_check     = true
  s3_force_path_style         = true
  skip_requesting_account_id  = true

  endpoints {
    s3             = "http://aws-localstack:4566"
    sns            = "http://aws-localstack:4566"
    sqs            = "http://aws-localstack:4566"
    dynamodb       = "http://aws-localstack:4566"
  }
}

variable "environment" {
  default = "local"
}


locals {
  environment = "${var.environment}"
  domain      = "system"
}

resource "aws_dynamodb_table" "system-db" {
  name           = "system-database"
  read_capacity  = 20
  write_capacity = 20
  hash_key       = "Id"
  range_key      = "Version"

  attribute {
    name = "Id"
    type = "S"
  }

  attribute {
    name = "Version"
    type = "N"
  }
}

resource "aws_sns_topic" "message-event-topic" {
  name = "${local.environment}-message-event-topic"
}

resource "aws_sns_topic" "user-event-topic" {
  name = "${local.environment}-user-event-topic"
}

# SQS Standard Queues
resource "aws_sqs_queue" "system-sqs-queue" {
  name                       = "${local.environment}-${local.domain}-sqs-queue"
  visibility_timeout_seconds = 45
}

resource "aws_sns_topic_subscription" "message-event-topic-subscription" {
  topic_arn            = "${aws_sns_topic.message-event-topic.arn}"
  protocol             = "sqs"
  endpoint             = "${aws_sqs_queue.system-sqs-queue.arn}"
  raw_message_delivery = true
}

resource "aws_sns_topic_subscription" "user-event-topic-subscription" {
  topic_arn            = "${aws_sns_topic.user-event-topic.arn}"
  protocol             = "sqs"
  endpoint             = "${aws_sqs_queue.system-sqs-queue.arn}"
  raw_message_delivery = true
}