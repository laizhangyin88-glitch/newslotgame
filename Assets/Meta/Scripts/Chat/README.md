- # Chat System
  Chat System을 사용하기 위한 도움 문서

  ## 핵심 Class
  - [Chat Messenger](#ChatMessenger)
  - [Chat Meta Manager](#ChatMetaManager)
  - [Chatting Controller](#ChattingController)
  - [Text Balloon](#TextBalloon)

  # ChatMessenger ([Link](/Assets/Meta/Scripts/Chat/ChatMessenger/ChatMessenger.cs))
  채팅 서버에 직접적으로 API를 호출하며 Channel 연결 및 구독을 관리하는 매니저 클래스 (싱글톤)

  연결된 Channel을 목록으로 유지하고 관리하며 주기적으로 Poll을 보내 메세지를 받아온다.

  채널에 구독을 하게되면 Connect, Disconnect, AddChat에 대한 정보를 받아올 수 있다.

  (단 해당 클래스는 ChannelType에 대한 정보는 없음)

  - __위치__: Splash Scene/Meta System/Systems/ChattingMessenger (GameObject)
  - __관련클래스:__ ChatMessengerChannel, IChatMessengerSubscriber, RequestChatPoll, IChatStrategy
  - __하는일:__ 
    - Connect, Discconect, Subscribe, Mute, Report, Poll,모든 ChatData 관리
    - Chat 보내기, 보낸 Chat Data 관리, Chat 정렬 순서 정의(IChatStrategy)

  

  # ChatMetaManager ([Link](/Assets/Meta/Scripts/Chat/ChatMetaManager/ChatMetaManager.cs))
  ChatMessenger에 ChannelType(Global, Club, Game)에 관련 된 행동을 처리하기 위한 매니저 클래스 (싱글톤)

  Game Logic에 의해 ChannelType에 해당하는 Channel Id가 변경되거나 사용이 불가능해 질 경우 처리하게된다.

  또한 ChannelType에 해당하는 Badge Count도 관리하게 된다.

  채널에 구독을 하게되면 Global Channel 변경, AddChat, Club Leave 에 대한 정보를 받아올 수 있다.


  - __위치__: Splash Scene/Meta System/Systems/ChattingMessenger (GameObject)
  - __관련클래스:__ IChatMetaListener
  - __하는일:__ 
    - ChannelType Validate, Change Global Channel, Profile 정렬 순서 정의


  # ChattingController ([Link](/Assets/Meta/Scripts/Chat/UI/Chatting/ChattingController.cs))
  Chatting UI의 필요한 행동을 처리하기 위한 컨트롤러 클래스 

  ChannelType이 변경 되거나 유저의 행동에 의해 Text Balloon을 다시 그리거나 Profile 목록을 갱신하는 행동을 처리합니다.

  - __위치__: Assets/Meta/App0/Prefabs/Lobby/In Game/Chat/Chatting (Prefab)
  - __관련클래스:__ ChannelChatBase, TextBalloonBase,  OSA_Chatting, OSA_ChatProfiles
  - __하는일:__
    - Open, Close, Change Channel Type, Update Profiles, Refesh Messages, 
    - Load Collecting Game, Send Chat Message

  # TextBalloon ([Link](/Assets/Meta/Scripts/Chat/UI/Chatting/TextBalloon/TextBalloonBase.cs))
  ChatPoll 데이터와 1:1로 대응되는 말풍선 UI.

  OSA로 관리되며 ChatType에 따라 각기 다른 Prefab이 존재하며 각각 다른 행동을 처리한다.

  - __위치__: Assets/Meta/App0/Prefabs/Lobby/In Game/Chat/Text Balloon (Prefab)
  - __관련클래스:__ OSA_Chatting
  - __하는일:__ Refresh , Others...