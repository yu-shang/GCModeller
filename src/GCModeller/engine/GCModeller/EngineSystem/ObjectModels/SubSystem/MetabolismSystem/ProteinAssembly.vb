﻿Imports System.Xml.Serialization
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services
Imports Microsoft.VisualBasic

Namespace EngineSystem.ObjectModels.SubSystem

    ''' <summary>
    ''' 所有的蛋白质复合物形成反应都是不可逆的，左端为Components，右端为ProteinComplexes
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ProteinAssembly : Inherits DelegateSystem
        Implements SystemsBiology.GCModeller.ModellingEngine.PlugIns.ISystemFrameworkEntry.ISystemFramework

        <DumpNode> <XmlElement> Public Property Proteins As EngineSystem.ObjectModels.Entity.Peptide()
        <DumpNode> Public Property ProteinComplex As EngineSystem.ObjectModels.Entity.Compound()
        <DumpNode> Protected PolypeptideDisposableInvokeSystem As SubSystem.DisposerSystem(Of Entity.Peptide)

        Sub New(Metabolism As SubSystem.MetabolismCompartment)
            Call MyBase.New(Metabolism)
        End Sub

        Public Overrides Function Initialize() As Integer
            Dim CellSystem = DirectCast(Me._CellComponentContainer, SubSystem.MetabolismCompartment)._CellSystem

            Me.Proteins = GetProteins()
            Call _SystemLogging.WriteLine(String.Format("Get {0} protein compounds", Me.Proteins.Count), "ProteinAssembly -> initialize()", Type:=Logging.MSG_TYPES.INF)

            If CellSystem.DataModel.ProteinAssemblies.IsNullOrEmpty Then
                MyBase._DynamicsExprs = New ObjectModels.Module.MetabolismFlux() {}
                Return -1
            Else
                Call _SystemLogging.WriteLine("Start to initialize the protein assembly equation model...")
                Dim LQuery = From assembly As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction
                             In CellSystem.DataModel.ProteinAssemblies
                             Let rxnList As ObjectModels.Module.MetabolismFlux = CreateDelegate(assembly, CellSystem.Metabolism.EnzymeKinetics, CellSystem.Metabolism.Metabolites, CellSystem.Metabolism.DelegateSystem.MetabolismEnzymes, SystemLogging)
                             Select rxnList '
                MyBase._DynamicsExprs = LQuery.ToArray.AddHandle.ToArray
                Call _SystemLogging.WriteLine("Protein assembly object model create job done!")
                Call _SystemLogging.WriteLine("start to initialize these equation model!")

                ProteinComplex = (From item In _DynamicsExprs Select CellSystem.Metabolism.Metabolites.GetItem(item.Identifier)).ToArray

#If DEBUG Then
                Dim InitLQuery = (From idx As Integer In NetworkComponents.Sequence Select NetworkComponents(idx).Initialize(CellSystem.Metabolism.Metabolites, SystemLogging)).ToArray 'link the flux object with the delegate system
#Else
                Dim InitLQuery = (From idx As Integer In NetworkComponents.Sequence.AsParallel Select NetworkComponents(idx).Initialize(CellSystem.Metabolism.Metabolites, SystemLogging)).ToArray 'link the flux object with the delegate system
#End If
                Call _SystemLogging.WriteLine("Protein assembly flux object models initialize completed with no error!")

                Me.PolypeptideDisposableInvokeSystem = New DisposerSystem(Of Entity.Peptide)(Me.Proteins, CellSystem.Metabolism, Entity.IDisposableCompound.DisposableCompoundTypes.Polypeptide)
                Call _SystemLogging.WriteLine("Start to initialize the disposal system...")
                Call Me.PolypeptideDisposableInvokeSystem.Initialize()
                Call _SystemLogging.WriteLine("disposal system initialize complete with no error!")

                Call CellSystem._InternalEventDriver.JoinEvents(PolypeptideDisposableInvokeSystem.NetworkComponents)
                Call CellSystem._InternalEventDriver.JoinEvents(Me._DynamicsExprs)

                Return 0
            End If
        End Function

        Private Shared Function ___createPolypeptideObject(Protein As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Polypeptide,
                                                           Metabolite As EngineSystem.ObjectModels.Entity.Compound) _
            As EngineSystem.ObjectModels.Entity.Peptide

            If Metabolite Is Nothing Then
                Call LoggingClient.WriteLine(String.Format("Could not found the data model for protein {0}!.", Protein.Identifier), "CreatePolypeptideObject()", Type:=Logging.MSG_TYPES.ERR)
            End If

            Dim Polypeptide As EngineSystem.ObjectModels.Entity.Peptide = New Entity.Peptide With {
                .Lamda = Protein.DynamicsLamda / (Protein.ProteinType + 1),
                .EntityBaseType = Metabolite,
                .ModelBaseElement = Metabolite.ModelBaseElement,
                .Quantity = Metabolite.DataSource.value,
                .Identifier = Metabolite.Identifier,
                .CompositionVector = Protein.CompositionVector.T
            }

            Return Polypeptide
        End Function

        Private Function GetProteins() As EngineSystem.ObjectModels.Entity.Compound()
            Dim CellSystem = DirectCast(Me._CellComponentContainer, SubSystem.MetabolismCompartment)._CellSystem
            Dim Metabolites = CellSystem.Metabolism.Metabolites
            Dim LQuery = From Protein In CellSystem.DataModel.Polypeptides Select ___createPolypeptideObject(Protein, Metabolites.GetItem(Protein.Identifier)) '
            Return LQuery.ToArray.AddHandle.ToArray
        End Function

        Public Overrides Function CreateServiceSerials() As Services.MySQL.IDataAcquisitionService()
            Dim CellSystem = DirectCast(Me._CellComponentContainer, SubSystem.MetabolismCompartment)._CellSystem
            Dim ServiceList = New List(Of Services.MySQL.IDataAcquisitionService) From {
                 New DataAcquisitionService(Adapter:=New EngineSystem.Services.DataAcquisition.DataAdapters.Proteome(Me), RuntimeContainer:=CellSystem.get_runtimeContainer)}
            Call ServiceList.AddRange(Me.PolypeptideDisposableInvokeSystem.CreateServiceSerials)
            Return ServiceList.ToArray
        End Function

        Public Overrides Sub MemoryDump(Dir As String)
            Call Me.I_CreateDump.SaveTo(String.Format("{0}/{1}.log", Dir, Me.GetType.Name))
        End Sub
    End Class

    Public Class RibosomalAssembly : Inherits ProteinAssembly

        Public Property RibosomalComplexes As EngineSystem.ObjectModels.Entity.Compound()

        Sub New(Metabolism As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
            Call MyBase.New(Metabolism)
        End Sub

        Public Overrides Function Initialize() As Integer
            Dim CellSystem = DirectCast(Me._CellComponentContainer, SubSystem.MetabolismCompartment)._CellSystem

            If CellSystem.DataModel.RibosomeAssembly.IsNullOrEmpty Then
                Call _SystemLogging.WriteLine("[Error] No ribosome assembly data was found, cell system structure is corrupted!", "RibosomalAssembly -> Initialize()", Type:=Logging.MSG_TYPES.ERR)
                Throw New DataException("[Error] No ribosome assembly data was found, cell system structure is corrupted!")
            Else
                Dim LQuery = From model As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction
                             In CellSystem.DataModel.RibosomeAssembly
                             Let rxn As ObjectModels.Module.MetabolismFlux = CreateDelegate(model, CellSystem.Metabolism.EnzymeKinetics, CellSystem.Metabolism.Metabolites, CellSystem.Metabolism.DelegateSystem.MetabolismEnzymes, SystemLogging)
                             Select rxn '
                MyBase._DynamicsExprs = LQuery.ToArray.AddHandle.ToArray
                Dim InitLQuery = (From idx As Integer In NetworkComponents.Sequence.AsParallel Select NetworkComponents(idx).Initialize(CellSystem.Metabolism.Metabolites, SystemLogging)).ToArray 'link the flux object with the delegate system

                Me.RibosomalComplexes = (From strId As String In (From item In CellSystem.DataModel.RibosomeAssembly Select item.Products.First.Identifier Distinct).ToArray Select CellSystem.Metabolism.Metabolites.GetItem(strId)).ToArray
                For Each Item As Entity.Compound In Me.RibosomalComplexes
                    Call _SystemLogging.WriteLine(String.Format("RibosomalComplexes(Subunit)  <- {0}", Item.Identifier))
                Next

                Call CellSystem._InternalEventDriver.JoinEvents(_DynamicsExprs)
            End If

            Return 0
        End Function
    End Class

    Public Class RNAPolymeraseAssembly : Inherits ProteinAssembly

        Public Property RNAPolymerase As EngineSystem.ObjectModels.Entity.Compound()

        Sub New(Metabolism As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
            Call MyBase.New(Metabolism)
        End Sub

        Public Overrides Function Initialize() As Integer
            Dim CellSystem = DirectCast(Me._CellComponentContainer, SubSystem.MetabolismCompartment)._CellSystem

            If CellSystem.DataModel.RibosomeAssembly.IsNullOrEmpty Then
                Call _SystemLogging.WriteLine("[Error] No ribosome assembly data was found, cell system structure is corrupted!", "RibosomalAssembly -> Initialize()", Type:=Logging.MSG_TYPES.ERR)
                Throw New DataException("[Error] No ribosome assembly data was found, cell system structure is corrupted!")
            Else
                Dim LQuery = From model As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction
                             In CellSystem.DataModel.RNAPolymerase
                             Let rxn As ObjectModels.Module.MetabolismFlux = CreateDelegate(model, CellSystem.Metabolism.EnzymeKinetics, CellSystem.Metabolism.Metabolites, CellSystem.Metabolism.DelegateSystem.MetabolismEnzymes, SystemLogging)
                             Select rxn '
                MyBase._DynamicsExprs = LQuery.ToArray.AddHandle.ToArray
                Dim InitLQuery = (From idx As Integer In NetworkComponents.Sequence.AsParallel Select NetworkComponents(idx).Initialize(CellSystem.Metabolism.Metabolites, SystemLogging)).ToArray 'link the flux object with the delegate system

                Me.RNAPolymerase = (From strId As String
                                    In (From item In CellSystem.DataModel.RNAPolymerase Select item.Products.First.Identifier Distinct)
                                    Select CellSystem.Metabolism.Metabolites.GetItem(strId)).ToArray

                Call CellSystem._InternalEventDriver.JoinEvents(_DynamicsExprs)
            End If

            Return 0
        End Function

        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer
            Return 0
        End Function
    End Class
End Namespace