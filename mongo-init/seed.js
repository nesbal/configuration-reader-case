db = db.getSiblingDB("ConfigurationDb");

db.Configurations.insertMany([
  {
    Name: "SiteName",
    Type: "string",
    Value: "soty.io",
    IsActive: true,
    ApplicationName: "SERVICE-A"
  },
  {
    Name: "IsBasketEnabled",
    Type: "bool",
    Value: "1",
    IsActive: true,
    ApplicationName: "SERVICE-B"
  },
  {
    Name: "MaxItemCount",
    Type: "int",
    Value: "50",
    IsActive: false,
    ApplicationName: "SERVICE-A"
  }
]);