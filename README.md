# 类功能简单说明

整个传送体系基于`Channel`构建, Channel充当了原先的实体管道, 每个`Channel`中有多个同类型物质的输入输出端口, 比如液体输入输出端口/气体输入输出端口
通过每个端口的`侧边栏`可以将建造的同类型端口配置到不同的`Channel`, 

`PortItem`是各种输入输出端口(固/液/气/辐射/电力)的`ViewModel`类

`SingleChannelController`是所有Channel的基类, 负责基础的PortItem管理, 如增删改名

`PortManager`是个单例, 负责管理Item和Channel之间的增删关系, 内部维护了一张 传送端口类型 -> (ChannelKey, ChannelList)的映射表

`PortChannelSideScreen`是端口的侧边栏菜单, 负责调整选中Item的信息, 如: 侧边栏标题/频道名修改/优先级调整/模式切换/候选词库/所加入的频道

`MyOverlayModes`是传送体系视图, 这里负责展示各种模式下的流向连线
