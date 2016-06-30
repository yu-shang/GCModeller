﻿
Namespace GridPBS

    ''' <summary>
    ''' 当需要进行多个虚拟细胞的互作计算分析的时候，需要使用这个对象所提供的方法来进行资源的统一协调调度。在进行计算的时候，每一个进程实例就是一个虚拟细胞对象，每一个进程实例之间都是使用Socket进行数据交换的
    ''' 数据交换的主要内容是所处的培养基环境的组分，故而本调配系统所要处理的具体问题就分为两个了：
    ''' 1. 每一个虚拟细胞对象之间的信号同步
    ''' 2. 处理培养基环境数据，为细胞之间的数据交换提供支持
    ''' 在进行数据交换的时候，首先将培养基环境数据序列化为一个CSV文件，然后在交换为内存映射文件用于数据交换。在服务器这里处理好数据之后再通过另外一个数据广播通道将新的培养基数据广播至每一个虚拟细胞实例进程之中
    ''' 
    ''' 现在假设细胞边缘的微环境的变化速率要大于培养基整体环境，故而每一次进程之间的数据交换的频率都是小于每一个进程之间的内核循环的速率的
    ''' 本并行计算的模块的工作原理是，每一个虚拟细胞进程为网络之中的一个节点，每一个节点都是自主运行的，每隔一段设定好的时间虚拟细胞进程就会主动的和本管理模块进行数据交换
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PBS : Implements IDriver_DataSource_Adapter(Of Double)

        Public Function get_DataSerials() As DataStorage.FileModel.DataSerials(Of Double)() Implements IDriver_DataSource_Adapter(Of Double).get_DataSerials
            Throw New NotImplementedException
        End Function

        Public Function get_ObjectHandlers() As DataStorage.FileModel.ObjectHandle() Implements IDriver_DataSource_Adapter(Of Double).get_ObjectHandlers
            Throw New NotImplementedException
        End Function
    End Class
End Namespace