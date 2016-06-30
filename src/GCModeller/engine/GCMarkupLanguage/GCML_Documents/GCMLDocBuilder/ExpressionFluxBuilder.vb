﻿Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.Assembly

Namespace Builder

    ''' <summary>
    ''' 表达流对象构建器，重建出目标模型的基因组、转录组
    ''' </summary>
    ''' <remarks>生成模型文件中的基因、转录单元和转录组分这三张表</remarks>
    Public Class ExpressionFluxBuilder : Inherits IBuilder

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <param name="Model">在模型对象之中的代谢组必须是已经构建好了的</param>
        ''' <remarks></remarks>
        Sub New(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel)
            MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel
            Printf("Start to compile the gene object in the metacyc.")
            Model.BacteriaGenome.Genes = CreateGenes(MetaCyc)
            Printf("Start to compile the transcript object in the metacyc.")
            Model.BacteriaGenome.Transcripts = CreateTranscripts(MetaCyc, Model)
            Printf("Start to compile transunit object in the metacyc.")
            Model.BacteriaGenome.TransUnits = CreateTransUnits(MetaCyc, Model)

            Return Model
        End Function

        ''' <summary>
        ''' 根据MetaCyc数据库模型生成转录单元对象列表
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function CreateTransUnits(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel) _
            As List(Of GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit)

            Dim TransUnits As GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit() = (From TransUnit As MetaCyc.File.DataFiles.Slots.TransUnit
                                                           In MetaCyc.GetTransUnits.AsParallel
                                                           Select GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit.CreateObject(TransUnit)).ToArray

            Printf("[INFO] function generate_transcript_units() create %s tu models.", TransUnits.Count)
            Printf("[INFO] link the transcript unit object with its gene cluster...")

            Dim LQuery = (From TU As Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit
                          In TransUnits.AsParallel
                          Let TU2 As GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit = TU.Link(Model.BacteriaGenome.Genes)
                          Select TU2
                          Order By TU2.Identifier Ascending).ToList
            Return LQuery
        End Function

        Private Shared Function CreateGenes(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) As GeneObject()
            Dim LQuery = From e As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Gene
                         In MetaCyc.GetGenes.AsParallel
                         Let G = GeneObject.CastTo(e)
                         Select G Order By G.Identifier Ascending   '
            Return LQuery.ToArray
        End Function

        ''' <summary>
        ''' 创建RNA分子对象，然后添加进入代谢组对象之中
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <param name="Model"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function CreateTranscripts(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel) As GCML_Documents.XmlElements.Bacterial_GENOME.Transcript()
            Dim List As List(Of GCML_Documents.XmlElements.Bacterial_GENOME.Transcript) = New List(Of GCML_Documents.XmlElements.Bacterial_GENOME.Transcript)
            Dim UnmodifiedProteins = GetAllUnmodifiedProduct(MetaCyc, Model)
            Dim LQuery = (From Gene In Model.BacteriaGenome.Genes Select Link(Gene, GCML_Documents.XmlElements.Bacterial_GENOME.Transcript.CreateObject(MetaCyc, Gene, Model), List)).ToArray   '

            '将构造出来的mRNA分子添加进入代谢组里面并添加句柄
            Dim n As Long = Model.Metabolism.Metabolites.Count
            Dim m As Long = (From tr In List Select tr.GenerateVector(MetaCyc)).ToArray.Sum
            Model.Metabolism.Metabolites.AddRange((From transcript In List Select transcript.CreateMetabolite).ToArray)

            Return List.ToArray
        End Function

        ''' <summary>
        ''' 将一个基因对象与相应的转录产物想联系起来
        ''' </summary>
        ''' <param name="Gene"></param>
        ''' <param name="Transcripts"></param>
        ''' <param name="List"></param>
        ''' <returns></returns>
        ''' <remarks>!!!请注意这里！！！</remarks>
        Private Shared Function Link(Gene As GeneObject, Transcripts As Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript(), List As List(Of GCML_Documents.XmlElements.Bacterial_GENOME.Transcript)) As Integer
            Dim Products As List(Of String) = New List(Of String)
            If Not Transcripts.IsNullOrEmpty Then
                Dim Transcript = Transcripts.First
                List.Add(Transcript)
            End If

            Return 0
        End Function

        ''' <summary>
        ''' 获取所有未经过化学修饰的蛋白质多肽链单体对象
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <param name="Model"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetAllUnmodifiedProduct(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel) As GCML_Documents.XmlElements.Metabolism.Metabolite()
            Dim LQuery = From Protein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein
                         In MetaCyc.GetProteins.AsParallel
                         Where Protein.IsPolypeptide AndAlso Not Protein.IsModifiedProtein AndAlso Not String.IsNullOrEmpty(Protein.Gene)
                         Select Protein.Identifier  '筛选条件为目标蛋白质对象为多肽链、不是化学修饰形式，并且基因号不为空
            Dim Proteins = LQuery.ToArray
            Return TakesMetabolites(UniqueIDCollection:=Proteins, Model:=Model)
        End Function

        Private Shared Function TakesMetabolites(UniqueIDCollection As Generic.IEnumerable(Of String), Model As Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel) As GCML_Documents.XmlElements.Metabolism.Metabolite()
            Dim LQuery = From UniqueID As String In UniqueIDCollection Select Model.Metabolism.Metabolites.GetItem(UniqueID) '
            Return LQuery.ToArray
        End Function
    End Class
End Namespace