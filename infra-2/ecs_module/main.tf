resource "aws_ecs_cluster" "ecs_fargate_cluster_ct" {
  name = var.ecs_cluster
}

resource "aws_iam_role" "ecsTaskExecutionRole" {
  name               = "ecsTaskExecutionRole"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Service = "ecs-tasks.amazonaws.com"
        }
        Action = "sts:AssumeRole"
      }
    ]
  })
}

resource "aws_iam_policy" "policies" {
  name        = "example-rds-fargate-policy"
  description = "Policy to allow RDS Fargate to access Secrets Manager and CloudWatch Logs"

  policy = jsonencode({
    Version   = "2012-10-17"
    Statement = [
      {
        Sid     = "AllowSMAccess"
        Action  = "secretsmanager:*",
        Effect  = "Allow"
        Resource = "*"
      },
      {
        Sid     = "AllowECRAccess"
        Action  = "ecr:*"
        Effect  = "Allow"
        Resource = "*"
      },
      {
        Sid     = "AllowCloudWatchLogs",
        Action  = [
          "logs:CreateLogGroup",
          "logs:CreateLogStream",
          "logs:PutLogEvents"
        ],
        Effect  = "Allow",
        Resource = "*"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "ecsTaskExecutionRole_policy" {
  role       = aws_iam_role.ecsTaskExecutionRole.name
  policy_arn = aws_iam_policy.policies.arn
}

resource "aws_ecs_task_definition" "business-motors_task" {
  family                   = "business-motors-task" # Naming our first task
  container_definitions    = <<DEFINITION
  [
    {
      "name": "business-motors-task",
      "image": "${var.ecr_repository_url}",
      "essential": true,
      "portMappings": [
        {
          "containerPort": 80,
          "hostPort": 80
        }
      ],
      "environment": [
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "${var.environment}"
        }
      ],
      "memory": 512,
      "cpu": 256,
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/business-motors-task",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "ecs"
        }
      }
    }
  ]
  DEFINITION
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"   
  memory                   = 512        
  cpu                      = 256        
  execution_role_arn       = "${aws_iam_role.ecsTaskExecutionRole.arn}"
}

resource "aws_ecs_service" "cmanager_ecs_service" {
  name            = "${aws_ecs_task_definition.business-motors_task.family}"                        
  cluster         = "${aws_ecs_cluster.ecs_fargate_cluster_ct.id}"             
  task_definition = "${aws_ecs_task_definition.business-motors_task.arn}" 
  # launch_type     = "FARGATE"
  desired_count   = 1

  deployment_controller {
    type = "ECS"
  }

  capacity_provider_strategy {
    capacity_provider = "FARGATE_SPOT"
    weight            = 1
  }

  load_balancer {
    target_group_arn = var.target_group_arn
    container_name   = "${aws_ecs_task_definition.business-motors_task.family}"
    container_port   = 80
  }

  network_configuration {
    subnets          = [var.default_subnet_a_id,
                        var.default_subnet_b_id,
                        var.default_subnet_c_id]
    assign_public_ip = true
    security_groups  = ["${aws_security_group.service_security_group.id}"]
  }
}

resource "aws_security_group" "service_security_group" {
  ingress {
    from_port = 0
    to_port   = 0
    protocol  = "-1"
    security_groups = [var.load_balancer_security_group_id]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}
