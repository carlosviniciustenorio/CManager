terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
  }

  backend "s3" {}
}

resource "aws_ecr_repository" "ecr_repo" {
  name = var.erc_repo_name
  lifecycle {
    prevent_destroy = true
  }
}

provider "aws" {
  region = var.aws_region
}