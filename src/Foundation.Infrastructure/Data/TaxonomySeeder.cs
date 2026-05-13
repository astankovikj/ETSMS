using Foundation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DomainEntity = Foundation.Domain.Entities.Domain;

namespace Foundation.Infrastructure.Data;

/// <summary>
/// Seeds the initial taxonomy data: categories, technologies, primary/secondary
/// relationships, and domains. Safe to run multiple times — check-before-insert
/// ensures no duplicates are created on subsequent executions (NFR-1).
/// </summary>
public static class TaxonomySeeder
{
    // ── 1. Category names ────────────────────────────────────────────────────────

    private static readonly string[] CategoryNames =
    [
        "Frontend",
        "Backend",
        "Database",
        "Cloud",
        "DevOps",
        "Mobile",
        "AI Tools",
    ];

    // ── 2. Technologies: (name, category) ────────────────────────────────────────

    private static readonly (string Name, string Category)[] TechnologyDefinitions =
    [
        // Frontend
        ("React",                   "Frontend"),
        ("Angular",                 "Frontend"),
        ("Vue.js",                  "Frontend"),
        ("Svelte",                  "Frontend"),
        ("Next.js",                 "Frontend"),
        ("Nuxt.js",                 "Frontend"),
        ("TypeScript",              "Frontend"),
        ("JavaScript",              "Frontend"),
        ("HTML",                    "Frontend"),
        ("CSS",                     "Frontend"),
        ("Sass/SCSS",               "Frontend"),
        ("Tailwind CSS",            "Frontend"),
        ("Bootstrap",               "Frontend"),
        ("Material UI",             "Frontend"),
        ("Webpack",                 "Frontend"),
        ("Vite",                    "Frontend"),
        ("GraphQL (Client)",        "Frontend"),
        ("Redux",                   "Frontend"),
        ("Storybook",               "Frontend"),
        ("Jest",                    "Frontend"),
        ("Cypress",                 "Frontend"),

        // Backend
        ("C#",                      "Backend"),
        (".NET / ASP.NET Core",     "Backend"),
        ("Node.js",                 "Backend"),
        ("Python",                  "Backend"),
        ("Java",                    "Backend"),
        ("Spring Boot",             "Backend"),
        ("Go",                      "Backend"),
        ("Rust",                    "Backend"),
        ("PHP",                     "Backend"),
        ("Laravel",                 "Backend"),
        ("Ruby",                    "Backend"),
        ("Ruby on Rails",           "Backend"),
        ("Kotlin",                  "Backend"),
        ("Scala",                   "Backend"),
        ("GraphQL (Server)",        "Backend"),
        ("REST API Design",         "Backend"),
        ("gRPC",                    "Backend"),
        ("FastAPI",                 "Backend"),
        ("Django",                  "Backend"),
        ("Express.js",              "Backend"),
        ("NestJS",                  "Backend"),

        // Database
        ("PostgreSQL",              "Database"),
        ("MySQL",                   "Database"),
        ("Microsoft SQL Server",    "Database"),
        ("SQLite",                  "Database"),
        ("Oracle Database",         "Database"),
        ("MongoDB",                 "Database"),
        ("Redis",                   "Database"),
        ("Elasticsearch",           "Database"),
        ("Cassandra",               "Database"),
        ("DynamoDB",                "Database"),
        ("Neo4j",                   "Database"),
        ("InfluxDB",                "Database"),
        ("Snowflake",               "Database"),
        ("BigQuery",                "Database"),
        ("Entity Framework Core",   "Database"),
        ("Dapper",                  "Database"),
        ("Flyway",                  "Database"),
        ("Liquibase",               "Database"),

        // Cloud
        ("AWS",                     "Cloud"),
        ("Azure",                   "Cloud"),
        ("Google Cloud Platform",   "Cloud"),
        ("AWS Lambda",              "Cloud"),
        ("Azure Functions",         "Cloud"),
        ("AWS S3",                  "Cloud"),
        ("Azure Blob Storage",      "Cloud"),
        ("AWS EC2",                 "Cloud"),
        ("Azure App Service",       "Cloud"),
        ("AWS EKS",                 "Cloud"),
        ("Azure Kubernetes Service","Cloud"),
        ("Google Kubernetes Engine","Cloud"),
        ("AWS RDS",                 "Cloud"),
        ("Azure SQL",               "Cloud"),
        ("Terraform",               "Cloud"),
        ("AWS CDK",                 "Cloud"),
        ("Pulumi",                  "Cloud"),
        ("AWS SQS",                 "Cloud"),
        ("Azure Service Bus",       "Cloud"),
        ("CloudFront",              "Cloud"),

        // DevOps
        ("Docker",                  "DevOps"),
        ("Kubernetes",              "DevOps"),
        ("Helm",                    "DevOps"),
        ("GitHub Actions",          "DevOps"),
        ("Azure DevOps",            "DevOps"),
        ("Jenkins",                 "DevOps"),
        ("CircleCI",                "DevOps"),
        ("ArgoCD",                  "DevOps"),
        ("Prometheus",              "DevOps"),
        ("Grafana",                 "DevOps"),
        ("Datadog",                 "DevOps"),
        ("New Relic",               "DevOps"),
        ("Nginx",                   "DevOps"),
        ("Istio",                   "DevOps"),
        ("Vault (HashiCorp)",       "DevOps"),

        // Mobile
        ("Swift",                   "Mobile"),
        ("Objective-C",             "Mobile"),
        ("SwiftUI",                 "Mobile"),
        ("Android (Kotlin)",        "Mobile"),
        ("Android (Java)",          "Mobile"),
        ("React Native",            "Mobile"),
        ("Flutter",                 "Mobile"),
        ("Xamarin",                 "Mobile"),
        ("MAUI",                    "Mobile"),
        ("Expo",                    "Mobile"),

        // AI Tools
        ("ChatGPT / OpenAI API",    "AI Tools"),
        ("GitHub Copilot",          "AI Tools"),
        ("Azure OpenAI Service",    "AI Tools"),
        ("Hugging Face",            "AI Tools"),
        ("LangChain",               "AI Tools"),
        ("LlamaIndex",              "AI Tools"),
        ("Ollama",                  "AI Tools"),
        ("Stable Diffusion",        "AI Tools"),
        ("Midjourney",              "AI Tools"),
        ("TensorFlow",              "AI Tools"),
        ("PyTorch",                 "AI Tools"),
        ("scikit-learn",            "AI Tools"),
        ("Keras",                   "AI Tools"),
        ("MLflow",                  "AI Tools"),
        ("Vertex AI",               "AI Tools"),
        ("Amazon SageMaker",        "AI Tools"),
        ("Semantic Kernel",         "AI Tools"),
        ("Prompt Engineering",      "AI Tools"),
    ];

    // ── 3. Primary → Secondary relationships: (primary name, secondary name) ─────

    private static readonly (string Primary, string Secondary)[] RelationshipDefinitions =
    [
        // Frontend family
        ("React",           "Next.js"),
        ("React",           "Redux"),
        ("React",           "Material UI"),
        ("Vue.js",          "Nuxt.js"),
        ("JavaScript",      "TypeScript"),
        ("JavaScript",      "React"),
        ("JavaScript",      "Angular"),
        ("JavaScript",      "Vue.js"),
        ("JavaScript",      "Svelte"),
        ("JavaScript",      "Express.js"),
        ("CSS",             "Sass/SCSS"),
        ("CSS",             "Tailwind CSS"),
        ("CSS",             "Bootstrap"),

        // Backend family
        ("C#",              ".NET / ASP.NET Core"),
        ("Python",          "Django"),
        ("Python",          "FastAPI"),
        ("Java",            "Spring Boot"),
        ("Ruby",            "Ruby on Rails"),
        ("PHP",             "Laravel"),
        ("Kotlin",          "Spring Boot"),
        ("Node.js",         "Express.js"),
        ("Node.js",         "NestJS"),

        // Database family
        ("PostgreSQL",      "Entity Framework Core"),
        ("PostgreSQL",      "Dapper"),
        ("Microsoft SQL Server", "Entity Framework Core"),
        ("Microsoft SQL Server", "Dapper"),

        // Cloud family
        ("AWS",             "AWS Lambda"),
        ("AWS",             "AWS S3"),
        ("AWS",             "AWS EC2"),
        ("AWS",             "AWS EKS"),
        ("AWS",             "AWS RDS"),
        ("AWS",             "AWS SQS"),
        ("AWS",             "AWS CDK"),
        ("AWS",             "CloudFront"),
        ("AWS",             "Amazon SageMaker"),
        ("Azure",           "Azure Functions"),
        ("Azure",           "Azure Blob Storage"),
        ("Azure",           "Azure App Service"),
        ("Azure",           "Azure Kubernetes Service"),
        ("Azure",           "Azure SQL"),
        ("Azure",           "Azure Service Bus"),
        ("Azure",           "Azure DevOps"),
        ("Azure",           "Azure OpenAI Service"),
        ("Google Cloud Platform", "Google Kubernetes Engine"),
        ("Google Cloud Platform", "BigQuery"),
        ("Google Cloud Platform", "Vertex AI"),

        // DevOps family
        ("Kubernetes",      "Helm"),
        ("Kubernetes",      "ArgoCD"),
        ("Kubernetes",      "Istio"),
        ("Docker",          "Kubernetes"),

        // Mobile family
        ("Swift",           "SwiftUI"),
        ("Android (Kotlin)","Android (Java)"),
        ("React",           "React Native"),
        ("React Native",    "Expo"),

        // AI Tools family
        ("TensorFlow",      "Keras"),
        ("ChatGPT / OpenAI API", "LangChain"),
        ("ChatGPT / OpenAI API", "Semantic Kernel"),
    ];

    // ── 4. Domains: (name, type) ─────────────────────────────────────────────────

    private static readonly (string Name, DomainType Type)[] DomainDefinitions =
    [
        // Industry
        ("Finance",               DomainType.Industry),
        ("Healthcare",            DomainType.Industry),
        ("Retail",                DomainType.Industry),
        ("Insurance",             DomainType.Industry),
        ("Education",             DomainType.Industry),
        ("Manufacturing",         DomainType.Industry),
        ("Energy & Utilities",    DomainType.Industry),
        ("Telecommunications",    DomainType.Industry),
        ("Government",            DomainType.Industry),
        ("Media & Entertainment", DomainType.Industry),

        // Business Function
        ("Engineering",           DomainType.BusinessFunction),
        ("HR",                    DomainType.BusinessFunction),
        ("Marketing",             DomainType.BusinessFunction),
        ("Sales",                 DomainType.BusinessFunction),
        ("Finance & Accounting",  DomainType.BusinessFunction),
        ("Operations",            DomainType.BusinessFunction),
        ("Legal & Compliance",    DomainType.BusinessFunction),
        ("Customer Support",      DomainType.BusinessFunction),
    ];

    // ── Public entry point ────────────────────────────────────────────────────────

    /// <summary>
    /// Seeds all taxonomy data in dependency order. Idempotent — safe to call on
    /// every application startup.
    /// </summary>
    public static async Task SeedAsync(FoundationDbContext db)
    {
        await SeedCategoriesAsync(db);
        await SeedTechnologiesAsync(db);
        await SeedRelationshipsAsync(db);
        await SeedDomainsAsync(db);
    }

    // ── Step 1: Categories ────────────────────────────────────────────────────────

    private static async Task SeedCategoriesAsync(FoundationDbContext db)
    {
        var existing = (await db.Categories
            .Select(c => c.Name)
            .ToListAsync())
            .ToHashSet();

        var toAdd = CategoryNames
            .Where(name => !existing.Contains(name))
            .Select(name => new Category { Name = name })
            .ToList();

        if (toAdd.Count > 0)
        {
            db.Categories.AddRange(toAdd);
            await db.SaveChangesAsync();
        }
    }

    // ── Step 2: Technologies ──────────────────────────────────────────────────────

    private static async Task SeedTechnologiesAsync(FoundationDbContext db)
    {
        var categoryLookup = await db.Categories
            .ToDictionaryAsync(c => c.Name, c => c.Id);

        var existingNames = (await db.Technologies
            .Select(t => t.Name)
            .ToListAsync())
            .ToHashSet();

        var toAdd = TechnologyDefinitions
            .Where(td => !existingNames.Contains(td.Name)
                      && categoryLookup.ContainsKey(td.Category))
            .Select(td => new Technology
            {
                Name       = td.Name,
                CategoryId = categoryLookup[td.Category],
            })
            .ToList();

        if (toAdd.Count > 0)
        {
            db.Technologies.AddRange(toAdd);
            await db.SaveChangesAsync();
        }
    }

    // ── Step 3: Primary / Secondary relationships ─────────────────────────────────

    private static async Task SeedRelationshipsAsync(FoundationDbContext db)
    {
        var techLookup = await db.Technologies
            .ToDictionaryAsync(t => t.Name, t => t.Id);

        var existingPairs = (await db.TechnologyRelationships
            .Select(r => new { r.PrimaryTechnologyId, r.SecondaryTechnologyId })
            .ToListAsync())
            .ToHashSet();

        var toAdd = new List<TechnologyRelationship>();

        foreach (var (primaryName, secondaryName) in RelationshipDefinitions)
        {
            if (!techLookup.TryGetValue(primaryName, out var primaryId)
             || !techLookup.TryGetValue(secondaryName, out var secondaryId))
            {
                // Skip if either technology was not found (graceful degradation).
                continue;
            }

            var pair = new { PrimaryTechnologyId = primaryId, SecondaryTechnologyId = secondaryId };
            if (!existingPairs.Contains(pair))
            {
                toAdd.Add(new TechnologyRelationship
                {
                    PrimaryTechnologyId   = primaryId,
                    SecondaryTechnologyId = secondaryId,
                });
            }
        }

        if (toAdd.Count > 0)
        {
            db.TechnologyRelationships.AddRange(toAdd);
            await db.SaveChangesAsync();
        }
    }

    // ── Step 4: Domains ───────────────────────────────────────────────────────────

    private static async Task SeedDomainsAsync(FoundationDbContext db)
    {
        var existing = (await db.Domains
            .Select(d => d.Name)
            .ToListAsync())
            .ToHashSet();

        var toAdd = DomainDefinitions
            .Where(dd => !existing.Contains(dd.Name))
            .Select(dd => new DomainEntity
            {
                Name       = dd.Name,
                DomainType = dd.Type,
            })
            .ToList();

        if (toAdd.Count > 0)
        {
            db.Domains.AddRange(toAdd);
            await db.SaveChangesAsync();
        }
    }
}
