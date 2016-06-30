﻿Imports LANS.SystemsBiology
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.MotifScans
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.Similarity.TOMQuery
Imports LANS.SystemsBiology.DatabaseServices.Regprecise
Imports LANS.SystemsBiology.DatabaseServices.Regprecise.WebServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq.Extensions

Partial Module CLI

    <ExportAPI("/Regulon.Test", Usage:="/Regulon.Test /in <meme.txt> /reg <genome.bbh.regulon.xml> /bbh <maps.bbh.Csv>")>
    Public Function RegulonTest(args As CommandLine) As Integer
        Dim inMEME As String = args("/in")
        Dim inRegulon As String = args("/reg")
        Dim inId As String = IO.Path.GetFileNameWithoutExtension(inMEME)
        Dim queryList = AnnotationModel.LoadDocument(inMEME)
        Dim source = inRegulon.LoadXml(Of BacteriaGenome)
        Dim sourceHash = (From x As Regulator
                          In source.Regulons.Regulators
                          Let uid As String = $"{x.LocusId}.{x.LocusTag.Value.Replace(":", "_")}"
                          Select uid, x)
        Dim target = (From x In sourceHash Where String.Equals(x.uid, inId, StringComparison.OrdinalIgnoreCase) Select x).FirstOrDefault
        Dim bbh As String = args("/bbh")
        Dim maps = bbh.LoadCsv(Of bbhMappings)
        Dim params As New Parameters
        Dim qResult As MotifHit() = Nothing
        Call __SWQueryCommon(inMEME, params, True, App.GetAppSysTempFile, qResult)
        Dim RegDb As Regulations = GCModeller.FileSystem.GetRegulations.LoadXml(Of Regulations)
        Dim GetRegulators = (From x As MotifHit
                             In qResult
                             Select x,
                                 regT = __getRegulators(x.Subject, RegDb)).ToArray
        Dim MapsRegulator = (From x In GetRegulators
                             Let regs = (From sId As String
                                         In x.regT
                                         Let map As bbhMappings = bbhMappings.GetQueryMaps(maps, sId)
                                         Where Not map Is Nothing
                                         Select map).ToArray
                             Where Not regs.IsNullOrEmpty
                             Select regs, x.x).ToArray
        Dim TF As String = target.x.LocusId
        Dim LQuery = (From x In MapsRegulator
                      Let getMap = (From xx As bbhMappings In x.regs
                                    Where String.Equals(TF, xx.hit_name)
                                    Select xx).FirstOrDefault
                      Where Not getMap Is Nothing
                      Select x.x,
                          getMap).ToArray
        If LQuery.IsNullOrEmpty Then
            Call $"{inId} is not support!".__DEBUG_ECHO
        Else
            For Each x In LQuery
                Call $"{x.x.ToString} supports {inId}!".__DEBUG_ECHO
            Next
        End If

        Return 0
    End Function

    Private Function __getRegulators(name As String, RegDb As Regulations) As String()
        Dim LDM = GCModeller.FileSystem.GetMotifLDM(name).LoadXml(Of AnnotationModel)
        Dim sites = LDM.Sites.ToArray(Function(site) site.Name)
        Dim regulators = sites.ToArray(Function(sId) RegDb.GetRegulators(sId)).MatrixToVector
        Return regulators
    End Function

    <ExportAPI("/Regulon.Reconstruct", Usage:="/Regulon.Reconstruct /bbh <bbh.csv> /genome <RegPrecise.genome.xml> /door <operon.door> [/out <outfile.csv>]")>
    Public Function RegulonReconstruct(args As CommandLine) As Integer
        Dim bbh As String = args("/bbh")
        Dim genome As String = args("/genome")
        Dim door As String = args("/door")
        Dim out As String = args.GetValue("/out", $"{bbh.TrimFileExt}.{IO.Path.GetFileNameWithoutExtension(genome)}.Regulons.Xml")
        Dim genomeGET = RegulonAPI.Reconstruct(bbh, genome, door)
        Return genomeGET.GetXml.SaveTo(out)
    End Function

    ''' <summary>
    ''' 其实bbh参数的数据类型不一定必须要严格满足bbh，只需要同时具备有query_name和hit_name就可以了
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Regulon.Reconstruct2", Usage:="/Regulon.Reconstruct2 /bbh <bbh.csv> /genome <RegPrecise.genome.DIR> /door <operons.opr> [/out <outDIR>]")>
    Public Function RegulonReconstructs2(args As CommandLine) As Integer
        Dim bbh As String = args("/bbh")
        Dim genome As String = args("/genome")
        Dim DOOR As String = args("/door")
        Dim out As String = args.GetValue("/out", bbh.TrimFileExt & ".Regulons/")
        Dim bbhValues = bbh.LoadCsv(Of NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit)
        Dim genomes = FileIO.FileSystem.GetFiles(genome, FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
        Dim doorOperon As LANS.SystemsBiology.Assembly.DOOR.DOOR
        If DOOR.FileExists Then
            doorOperon = Assembly.DOOR.Load(DOOR)
        Else
            doorOperon = Assembly.DOOR.DOOR.CreateEmpty
        End If
        Dim mapHash = bbhValues.BuildMapHash
        Dim LQuery = (From x As String In genomes
                      Let regulators = RegulonAPI.Reconstruct(mapHash, x.LoadXml(Of BacteriaGenome), doorOperon)
                      Where Not regulators.IsNullOrEmpty
                      Let id As String = IO.Path.GetFileNameWithoutExtension(x)
                      Select id, _genome = New BacteriaGenome With {
                          .Regulons = New DatabaseServices.Regprecise.Regulon With {
                                .Regulators = regulators
                          },
                          .BacteriaGenome = New WebServices.JSONLDM.genome With {
                                .name = "@" & id}}).ToArray

        For Each _genome In LQuery
            Dim path As String = $"{out}/{_genome.id}.xml"
            Call _genome._genome.GetXml.SaveTo(path)
        Next

        Return 0
    End Function

    ''' <summary>
    ''' 进行Regulon的批量重建工作
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Regulon.Reconstructs",
               Info:="Doing the regulon reconstruction job in batch mode.",
               Usage:="/Regulon.Reconstructs /bbh <bbh_EXPORT_csv.DIR> /genome <RegPrecise.genome.DIR> [/door <operon.door> /out <outDIR>]")>
    <ParameterInfo("/bbh", False, Description:="A directory which contains the bbh export csv data from the localblast tool.")>
    <ParameterInfo("/genome", False, Description:="The directory which contains the RegPrecise bacterial genome downloads data from the RegPrecise web server.")>
    <ParameterInfo("/door", False, Description:="Door file which is the prediction data of the bacterial operon.")>
    Public Function RegulonReconstructs(args As CommandLine) As Integer
        Dim inDIR As String = args("/bbh")
        Dim genomeDIR As String = args("/genome")
        Dim door As String = args("/door")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & "/Regulons")
        Dim bbh As Dictionary(Of String, String) = inDIR.LoadSourceEntryList({"*.csv"}, True)
        Dim genomes As Dictionary(Of String, String) = genomeDIR.LoadSourceEntryList({"*.xml"}, True)
        Dim pairs = (From x As KeyValuePair(Of String, String) In genomes
                     Let paired As String = __pairs(x.Key, bbh)
                     Where Not String.IsNullOrEmpty(paired)
                     Select bbhMapped = paired,
                         genome = x.Value).ToArray
        Dim doorOperon As LANS.SystemsBiology.Assembly.DOOR.DOOR
        If door.FileExists Then
            doorOperon = Assembly.DOOR.Load(door)
        Else
            doorOperon = Assembly.DOOR.DOOR.CreateEmpty
        End If
        Dim LQuery = (From x In pairs.AsParallel
                      Select x,
                          genome = RegulonAPI.Reconstruct(x.bbhMapped, x.genome, doorOperon)).ToArray

        For Each genome In LQuery
            Dim path As String = $"{out}/{genome.genome.BacteriaGenome.name.NormalizePathString(True)}.xml"
            Call genome.genome.GetXml.SaveTo(path)
        Next

        Return 0
    End Function

    Private Function __pairs(genome As String, bbh As Dictionary(Of String, String)) As String
        genome = genome.Replace(" ", "_")
        Dim LQuery = (From x In bbh Where InStr(x.Key, genome, CompareMethod.Text) > 0 Select x.Value).FirstOrDefault
        Return LQuery
    End Function

    <ExportAPI("/regulon.export", Usage:="/regulon.export /in <sw-tom_out.DIR> /ref <regulon.bbh.xml.DIR> [/out <out.csv>]")>
    Public Function ExportRegulon(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim refDIR As String = args("/ref")
        Dim out As String = args.GetValue("/out", inDIR & ".Csv")
        Dim result = RegulonDef.Export(refDIR, inDIR)
        Call result.SaveTo(out)
        result = (From x In result Where InStr(x.Hit, x.Family, CompareMethod.Text) > 0 Select x).ToArray
        out = out.TrimFileExt & ".2.Csv"
        Return result.SaveTo(out).CLICode
    End Function
End Module