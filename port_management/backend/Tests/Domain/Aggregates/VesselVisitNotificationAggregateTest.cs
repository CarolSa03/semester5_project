// using FluentAssertions;
// using PortManagement.Domain.Vessel.Entities;
// using PortManagement.Domain.Vessel.Enums;
// using PortManagement.Domain.Vessel.ValueObjects;
// using PortManagement.Domain.ShippingAgent.Entities;
// using Xunit;

// namespace PortManagement.Tests.Domain.Aggregates;

// /// <summary>
// /// Integration tests at aggregate level
// /// Tests the interaction between VesselVisitNotification and its collaborating entities
// /// (CargoManifest, CrewInfo, VesselRecord)
// /// 
// /// This tests the business workflow where multiple domain objects work together
// /// </summary>
// public class VesselVisitNotificationAggregateTest
// {
//     #region Complete workflow tests

//     [Fact]
//     public void FluxoCompleto_NotificacaoComCargoPerigoso_DeveExigirInformacaoDeCrew()
//     {
//         // Arrange - Create notification with dangerous cargo
//         var notification = CreateNotificationWithDangerousCargo();

//         // Act - Check if crew info is required
//         var requiresCrew = notification.RequiresCrewInfoForSecurity();

//         // Assert
//         requiresCrew.Should().BeTrue("dangerous cargo requires crew information");
//     }

//     [Fact]
//     public void FluxoCompleto_ValidacaoDeSubmissao_DeveVerificarTodasAsRegrasDeNegocio()
//     {
//         // Arrange - Valid notification ready for submission
//         var notification = CreateCompleteValidNotification();

//         // Act - Validate for submission (should check all business rules)
//         Action act = () => notification.ValidateForSubmission();

//         // Assert - Should not throw (all validations pass)
//         act.Should().NotThrow();
//     }

//     #endregion

//     #region Cargo manifest interaction tests

//     [Fact]
//     public void Interacao_NotificacaoComMultiplosManifests_DeveValidarTodos()
//     {
//         // Arrange - Notification with multiple manifests (loading and unloading)
//         var notification = CreateNotificationWithMultipleManifests();

//         // Act
//         var hasDangerousCargo = notification.RequiresCrewInfoForSecurity();

//         // Assert
//         hasDangerousCargo.Should().BeTrue("one of the manifests contains dangerous cargo");
//     }

//     [Fact]
//     public void Interacao_CargoManifestInvalido_DeveFalharValidacaoDeNotificacao()
//     {
//         // Arrange - Notification with invalid cargo manifest
//         var notification = CreateCompleteValidNotification();
//         notification.CargoManifests![0].Containers![0].ContainerId = null; // Make it invalid

//         // Act
//         Action act = () => notification.CargoManifests[0].Validate();

//         // Assert - CargoManifest validation should fail
//         act.Should().Throw<InvalidOperationException>()
//             .WithMessage("Container ID is required");
//     }

//     #endregion

//     #region Crew info interaction tests

//     [Fact]
//     public void Interacao_NotificacaoComCrewInfoInvalido_DeveFalharValidacaoDeCrew()
//     {
//         // Arrange
//         var notification = CreateNotificationWithDangerousCargo();
//         notification.Crew = new CrewInfo
//         {
//             CaptainName = null, // Invalid
//             CrewCount = 25,
//             SafetyOfficers = new List<CrewMember>()
//         };

//         // Act
//         Action actCrewValidation = () => notification.Crew!.Validate();

//         // Assert
//         actCrewValidation.Should().Throw<InvalidOperationException>();
//     }

//     #endregion

//     #region Vessel record interaction tests

//     [Fact]
//     public void Interacao_NotificacaoSemVessel_DeveFalharValidacaoDeSubmissao()
//     {
//         // Arrange
//         var notification = CreateCompleteValidNotification();
//         notification.Vessel = null;

//         // Act
//         Action act = () => notification.ValidateForSubmission();

//         // Assert
//         act.Should().Throw<InvalidOperationException>()
//             .WithMessage("Vessel information is required");
//     }

//     #endregion

//     #region Helper methods

//     private VesselVisitNotification CreateCompleteValidNotification()
//     {
//         var businessId = VesselVisitBusinessId.Parse("2025-PORTO-000001");
//         var notification = new VesselVisitNotification(businessId);

//         // Create vessel with value objects
//         notification.Vessel = new VesselRecord(
//             new Imo(9074729),
//             new VRName("Test Vessel"),
//             new VesselType { Name = "Container Ship", CapacityTEU = 5000 },
//             new VROwner("Test Owner")
//         );

//         notification.ShippingAgentRepresentative = new ShippingAgentRepresentative
//         {
//             Id = 1,
//             Name = "John Agent",
//             Email = "agent@shipping.com"
//         };

//         notification.ETA = DateTime.UtcNow.AddDays(2);
//         notification.ETD = DateTime.UtcNow.AddDays(4);
//         notification.Status = VesselNotificationStatus.InProgress;

//         notification.CargoManifests = new List<CargoManifest>
//         {
//             new CargoManifest
//             {
//                 Type = ManifestType.Loading,
//                 Containers = new List<ContainerInfo>
//                 {
//                     new ContainerInfo
//                     {
//                         ContainerId = new CMContainerId("MSCU1234567"),
//                         CargoType = new CMCargoType("General"),
//                         Bay = 1, Row = 1, Tier = 1
//                     }
//                 }
//             }
//         };

//         return notification;
//     }

//     private VesselVisitNotification CreateNotificationWithDangerousCargo()
//     {
//         var notification = CreateCompleteValidNotification();
//         notification.CargoManifests = new List<CargoManifest>
//         {
//             new CargoManifest
//             {
//                 Type = ManifestType.Loading,
//                 Containers = new List<ContainerInfo>
//                 {
//                     new ContainerInfo
//                     {
//                         ContainerId = new CMContainerId("MSCU1234567"),
//                         CargoType = new CMCargoType("HAZMAT"),
//                         Bay = 1, Row = 1, Tier = 1
//                     }
//                 }
//             }
//         };
//         return notification;
//     }

//     private VesselVisitNotification CreateNotificationWithMultipleManifests()
//     {
//         var notification = CreateCompleteValidNotification();
//         notification.CargoManifests = new List<CargoManifest>
//         {
//             new CargoManifest
//             {
//                 Type = ManifestType.Unloading,
//                 Containers = new List<ContainerInfo>
//                 {
//                     new ContainerInfo
//                     {
//                         ContainerId = new CMContainerId("MSCU1111111"),
//                         CargoType = new CMCargoType("General"),
//                         Bay = 1, Row = 1, Tier = 1
//                     }
//                 }
//             },
//             new CargoManifest
//             {
//                 Type = ManifestType.Loading,
//                 Containers = new List<ContainerInfo>
//                 {
//                     new ContainerInfo
//                     {
//                         ContainerId = new CMContainerId("MSCU2222222"),
//                         CargoType = new CMCargoType("CHEMICAL"),
//                         Bay = 2, Row = 2, Tier = 2
//                     }
//                 }
//             }
//         };
//         return notification;
//     }

//     #endregion
// }

