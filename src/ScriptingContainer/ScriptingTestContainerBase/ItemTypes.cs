using System.ComponentModel;
using System;
using TCatSysManagerLib;


namespace TwinCAT.SystemManager
{
	

	/// <summary>
	/// Tree Item Type for the Tree Item
	/// </summary>
	/// <remarks>This is the CLS-compliant, corresponding type to TCatSysManagerLibs TREEITEMTYPE</remarks>
	public enum TreeItemType
	{
		/// <summary>
		/// NotProcessed Tree Item object (Uninitialized, 0)
		/// </summary>
		[Description("Uninitialized / Unknown item.")]
		None = TREEITEMTYPES.TREEITEMTYPE_UNKNOWN,
		/// <summary>
		/// System configuration Tree Item ("SYSTEM-Konfiguration") TIRC (Has no valid Type at the moment == 0)
		/// </summary>
		[Description("System configuration Item.")]
		SystemConfiguration = 0,
		/// <summary>
		/// Task item (Task, 1)
		/// </summary>
		[Description("Task item.")]
		Task = TREEITEMTYPES.TREEITEMTYPE_TASK,
		/// <summary>
		/// Device item (Device, 2)
		/// </summary>
		[Description("Device item.")]
		Device = TREEITEMTYPES.TREEITEMTYPE_DEVICE,
		/// <summary>
		/// Process-Image (Image, 3)
		/// </summary>
		[Description("Process-Image item.")]
		Image = TREEITEMTYPES.TREEITEMTYPE_IMAGE,
		/// <summary>
		/// Mapping (Mapping, 4)
		/// </summary>
		[Description("Mapping item.")]
		Mapping = TREEITEMTYPES.TREEITEMTYPE_MAPPING,
		/// <summary>
		/// Box item (Box, 5)
		/// </summary>
		[Description("Box item.")]
		Box = TREEITEMTYPES.TREEITEMTYPE_BOX,

		/// <summary>
		/// Terminal item (Term, 6)
		/// </summary>
		[Description("Terminal item.")]
		Terminal = TREEITEMTYPES.TREEITEMTYPE_TERM,
		/// <summary>
		/// Variable / Symbol item (Var, 7)
		/// </summary>
		[Description("Variable / Symbol item.")]
		Variable = TREEITEMTYPES.TREEITEMTYPE_VAR,
		/// <summary>
		/// Variable group item (VarGrp, 8) or Channels in Terminals
		/// </summary>

		[Description("Variable group item or Channels in Terminals")]
		VariableGroup = TREEITEMTYPES.TREEITEMTYPE_VARGRP,
		/// <summary>
		/// PlcControl project item (IecPrj, 9)
		/// </summary>
		
		[Description("PlcControl project item.")]
		IecProject = TREEITEMTYPES.TREEITEMTYPE_IECPRJ,
		/// <summary>
		/// Cnc Project Item. (CncPrj, 10)
		/// </summary>
		
		[Description("Cnc Project Item.")]
		CncProject = TREEITEMTYPES.TREEITEMTYPE_CNCPRJ,
		/// <summary>
		/// GSD Described Module
		/// </summary>

		[Description("GSD Description Module.")]
		GsdModule = TREEITEMTYPES.TREEITEMTYPE_GSDMOD,
		/// <summary>
		/// CDL Item.
		/// </summary>
		
		[Description("CDL")]
		Cdl = TREEITEMTYPES.TREEITEMTYPE_CDL,
		/// <summary>
		/// PLC Runtime item (IecLzs, 13)
		/// </summary>
		[Description("PLC Runtime item")]
		PlcRuntime = TREEITEMTYPES.TREEITEMTYPE_IECLZS,
		/// <summary>
		/// 
		/// </summary>
		[Obsolete("User PlcRuntime instead,false")]
		PlcProject = 13,
		/// <summary>
		/// Plc configuration (LzsGrp)
		/// </summary>
		
		[Description("Plc configuration Item.")]
		PlcConfiguration = TREEITEMTYPES.TREEITEMTYPE_LZSGRP,
		/// <summary>
		/// IO configuration
		/// </summary>
		
		[Description("IO configuration Item.")]
		IOConfiguration = TREEITEMTYPES.TREEITEMTYPE_IODEF,
		/// <summary>
		/// Additional Tasks (AddTasks, 16)
		/// </summary>
		
		[Description("Additional Tasks Item.")]
		AdditionalTasks = TREEITEMTYPES.TREEITEMTYPE_ADDTASKS,
		/// <summary>
		/// Device Group Item
		/// </summary>
		
		[Description("Device Group Item.")]
		DeviceGroup = TREEITEMTYPES.TREEITEMTYPE_DEVICEGRP,
		/// <summary>
		/// Map Group
		/// </summary>
		
		[Description("Map Group Item.")]
		MapGroup = TREEITEMTYPES.TREEITEMTYPE_MAPGRP,
		/// <summary>
		/// NC Configuration (NCDEF, 19)
		/// </summary>
		
		[Description("NC Configuration Item")]
		NCConfiguration = TREEITEMTYPES.TREEITEMTYPE_NCDEF,
		/// <summary>
		/// NC Axes
		/// </summary>

		[Description("NC Axes item.")]
		[Obsolete("Not supported, use TreeItemType.NCChannel instead!",true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		NCAxes = TREEITEMTYPES.TREEITEMTYPE_NCAXISES,

		/// <summary>
		/// NC Channel (e.g. the Node 'Axes') (NCChannel, 21)
		/// </summary>
		[Description("NC Channel (e.g. the Node 'Axes') Item.")]
		NCChannel = 21,
		/// <summary>
		/// NC Axis
		/// </summary>
		[Description("NC Axis Item.")]
		NCAxis = 22,
		/// <summary>
		/// Axis Encoder (ID: 23)
		/// </summary>
		[Description("Axis Encoder Item.")]
		NCEncoder = 23,
		/// <summary>
		/// Axis drive (ID: 24)
		/// </summary>
		NCDrive = 24,
		/// <summary>
		/// Axis controller
		/// </summary>
		[Description("Axis controller Item.")]
		NCController = 25,
		/// <summary>
		/// NC Group
		/// </summary>
		
		[Description("NC Group Item.")]
		NCGroup = 26,
		/// <summary>
		/// NC Interpreter
		/// </summary>
		
		[Description("NC Interpreter Item")]
		NCInterpreter = 27,
		/// <summary>
		/// CanPDO
		/// </summary>
		
		[Description("CanPDO Item.")]
		CanPdo = 30,
		/// <summary>
		///Real time Settings ATTENTION THE TYPE IS IDENTICAL TO RouteSettings(RTimeSet, 31)
		/// </summary>
		
		[Description("Real time Settings Item.)")]
		RealTimeSettings = 31,
		/// <summary>
		/// Route Settings (RTimeSet, 31)
		/// </summary>
		
		[Description("Route Settings Item.")]
		RouteSettings = 31,
		/// <summary>
		/// PLC Variables of the BC
		/// </summary>
		
		[Description("PLC Variables Item of the BC")]
		BcPlcVariables = 32,
		/// <summary>
		/// File Name item.
		/// </summary>
		
		[Description("File Name Item.")]
		FileName = 33,
		/// <summary>
		/// DnetConnect.
		/// </summary>
		
		[Description("DnetConnect Item.")]
		DnetConnect = 34,
		/// <summary>
		/// Network variable publisher
		/// </summary>
		[Description("Network variable publisher Item.")]
		NVPublisherVar = 35,
		/// <summary>
		/// Network variable subscriber
		/// </summary>
		[Description("Network variable subscriber Item.")]
		NVSubscriberVar = 36,
		/// <summary>
		/// FlbCmd
		/// </summary>
		
		[Description("FlbCmd Item.")]
		FlbCmd = 37,
		/// <summary>
		/// NC Table Group Item
		/// </summary>
		
		[Description("NC Table Group Item.")]
		NCTableGroup = 40,
		/// <summary>
		/// NC Table
		/// </summary>
		
		[Description("NC Table Item.")]
		NCTable = 41,
		/// <summary>
		/// NC Table Slave Item.
		/// </summary>
		
		[Description("NC Table Slave Item.")]
		NCTableSlave = 42,

		/// <summary>
		/// EipConnection item.
		/// </summary>
		[Description("EipConnection Item.")]
		EipConnection = 43,
		/// <summary>
		/// PnIoApi Item.
		/// </summary>
		[Description("PnIoApi Item.")]
		PnIoApi = 44,
		/// <summary>
		/// PnIoMod Item.
		/// </summary>
		[Description("PnIoMod Item.")]
		PnIoMod = 45,
		/// <summary>
		/// PnIoSubMod item.
		/// </summary>
		[Description("PnIoSubMod Item.")]
		PnIoSubMod = 46,
		/// <summary>
		/// Ethernet Protocol Item.
		/// </summary>
		[Description("Ethernet Protocol Item.")]
		EthernetProtocol = 47,
		/// <summary>
		/// TCOM object (48)
		/// </summary>x0d\x0a
		[Description("TCOM object Item.")]
		TComObject = 48,
		/// <summary>
		/// TCOM Object group (49)
		/// </summary>
		[Description("TCOM Object group Item.")]
		TComObjectGroup = 49,
		/// <summary>
		/// UdpIpSendData Item.
		/// </summary>
		[Description("UdpIpSendData Item.")]
		UdpIpSendData = 50,
		/// <summary>
		/// UdpIpReceiveData Item.
		/// </summary>
		[Description("UdpIpReceiveData Item.")]
		UdpIpReceiveData = 51,

		/// <summary>
		/// 
		/// </summary>
		TComMapping = 52,
		/// <summary>
		/// 
		/// </summary>
		ModuleCfgEditGrp = 53,
		/// <summary>
		/// 
		/// </summary>
		ModuleCfgEdit = 54,
		/// <summary>
		/// 
		/// </summary>
		CCatSlot = 55,
		/// <summary>
		/// Nested Plc Project (56)
		/// </summary>
		PlcProjectDef = TREEITEMTYPES.TREEITEMTYPE_PLCPROJECTDEF,
		/// <summary>
		/// 
		/// </summary>
		TComPlcObject = 57,
		/// <summary>
		/// 
		/// </summary>
		HierarchyNode = 58,
		/// <summary>
		/// 
		/// </summary>
		License = 59,

		/// <summary>
		/// "TIAC" CAM Configuration
		/// </summary>
		/// <remarks>Path: "TIAC"</remarks>
		[Description("CAM Configuration Item.")]
		CamDef = 200,
		/// <summary>
		/// CamGroup item.
		/// </summary>
		
		[Description("CamGroup Item.")]
		CamGroup = 201,
		/// <summary>
		/// Cam Item.
		/// </summary>
		
		[Description("Cam Item.")]
		Cam = 202,
		/// <summary>
		/// CamEncoder Item.
		/// </summary>
		
		[Description("CamEncoder Item.")]
		CamEncoder = 203,
		/// <summary>
		/// CamToolGroup Item.
		/// </summary>
		
		[Description("CamToolGroup Item.")]
		CamToolGroup = 204,
		/// <summary>
		/// CamTool Item.
		/// </summary>
		
		[Description("CamTool Item.")]
		CamTool = 205,
		/// <summary>
		/// LineDef Item.
		/// </summary>
		
		[Description("LineDef Item.")]
		LineDef = 300,
		/// <summary>
		/// CNC Configuration (400)
		/// </summary>
		
		[Description("CNC Configuration Item.")]
		CncConfiguration = 400,
		/// <summary>
		/// ISG Channel item
		/// </summary>
		
		[Description("ISG Channel Item")]
		CncChannel = 401,
		/// <summary>
		/// CNC Axis Group Item
		/// </summary>
		
		[Description("CNC Axis Group Item")]
		CncAxisGroup = 402,
		/// <summary>
		/// CNC Axis (ISG, 403)
		/// </summary>
		
		[Description("CNC Axis Item.")]
		CncAxis = 403,

		/// <summary>
		/// 
		/// </summary>
		RtsConfig = 500,
		/// <summary>
		/// 
		/// </summary>
		RtsApp = 501,
		/// <summary>
		/// 
		/// </summary>
		RtsAppTask = 502,
		/// <summary>
		/// 
		/// </summary>
		RtsAdi = 503,
		/// <summary>
		/// 
		/// </summary>
		CppConfig = 504,
		/// <summary>
		/// 
		/// </summary>
		SplcConfig = 505,

		/// <summary>
		/// Plc Application (Root Plc Object, 600) 
		/// </summary>
		PlcApplication = TREEITEMTYPES.TREEITEMTYPE_PLCAPP,
		/// <summary>
		/// Plc Folder object (601)
		/// </summary>
		PlcFolder = TREEITEMTYPES.TREEITEMTYPE_PLCFOLDER,

		/// <summary>
		/// Plc POU Program (602)
		/// </summary>
		PlcPouProgram = TREEITEMTYPES.TREEITEMTYPE_PLCPOUPROG,
		/// <summary>
		/// Plc POU Function (603)
		/// </summary>
		PlcPouFunction = TREEITEMTYPES.TREEITEMTYPE_PLCPOUFUNC,
		/// <summary>
		/// Plc POU Function Block (604)
		/// </summary>
		PlcPouFunctionBlock = TREEITEMTYPES.TREEITEMTYPE_PLCPOUFB,

		/// <summary>
		/// Plc Enum Datatype (605)
		/// </summary>
		PlcDutEnum = TREEITEMTYPES.TREEITEMTYPE_PLCDUTENUM,
		/// <summary>
		/// Plc Struct DataType (606)
		/// </summary>
		PlcDutStruct = TREEITEMTYPES.TREEITEMTYPE_PLCDUTSTRUCT,
		/// <summary>
		/// Plc Union DataType (607)
		/// </summary>
		PlcDutUnion = TREEITEMTYPES.TREEITEMTYPE_PLCDUTUNION,

		/// <summary>
		/// Plc Action (608)
		/// </summary>
		PlcAction = TREEITEMTYPES.TREEITEMTYPE_PLCACTION,
		/// <summary>
		/// Plc Method (609)
		/// </summary>
		PlcMethod = TREEITEMTYPES.TREEITEMTYPE_PLCMETHOD,
		/// <summary>
		/// Plc Interface Method (610)
		/// </summary>
		PlcItfMethod = TREEITEMTYPES.TREEITEMTYPE_PLCITFMETH,
		/// <summary>
		/// Plc Property (611)
		/// </summary>
		PlcProperty = TREEITEMTYPES.TREEITEMTYPE_PLCPROP,
		/// <summary>
		/// Plc InterfaceProperty (612)
		/// </summary>
		PlcItfProperty = TREEITEMTYPES.TREEITEMTYPE_PLCITFPROP,
		/// <summary>
		/// Plc Property Getter (613)
		/// </summary>
		PlcPropertyGet = TREEITEMTYPES.TREEITEMTYPE_PLCPROPGET,
		/// <summary>
		/// Plc Property Setter (614)
		/// </summary>
		PlcPropertySet = TREEITEMTYPES.TREEITEMTYPE_PLCPROPSET,
		/// <summary>
		/// Plc Global Variables List (615)
		/// </summary>
		PlcGvl = TREEITEMTYPES.TREEITEMTYPE_PLCGVL,
		/// <summary>
		/// Plc Transient Object (616)
		/// </summary>
		PlcTransition = TREEITEMTYPES.TREEITEMTYPE_PLCTRANSITION,
		/// <summary>
		/// Plc Library Manager (617)
		/// </summary>
		PlcLibraryManager = TREEITEMTYPES.TREEITEMTYPE_PLCLIBMAN,
		/// <summary>
		/// Plc Interface (618)
		/// </summary>
		PlcInterface = TREEITEMTYPES.TREEITEMTYPE_PLCITF,
		/// <summary>
		/// Plc Visual Object (619)
		/// </summary>
		PlcVisualObject = TREEITEMTYPES.TREEITEMTYPE_PLCVISOBJ,
		/// <summary>
		/// Plc Visual Manager (620)
		/// </summary>
		PlcVisualManager = TREEITEMTYPES.TREEITEMTYPE_PLCVISMAN,

		/// <summary>
		/// Plc Task object (621)
		/// </summary>
		PlcTask = TREEITEMTYPES.TREEITEMTYPE_PLCTASK,

		///// <summary>
		///// Plc Project Information (622)
		///// </summary>
		//PlcProjectInfo = TREEITEMTYPES.TREEITEMTYPE_PLCPROJECTINFO,

		 /// <summary>
		/// Plc DataType Alias (623)
		/// </summary>
		PlcDutAlias = TREEITEMTYPES.TREEITEMTYPE_PLCDUTALIAS,

		/// <summary>
		/// Target Visu (624)
		/// </summary>
		PlcTargetVisualization = TREEITEMTYPES.TREEITEMTYPE_PLCTARGETVISU,		// TargetVisualization
		/// <summary>
		/// Global Text List (625)
		/// </summary>
		PlcGlobalTextList = TREEITEMTYPES.TREEITEMTYPE_PLCGLOBALTEXTLIST,	// Global Text List for Visu
		/// <summary>
		/// Text List (626)
		/// </summary>
		PlcTextList = TREEITEMTYPES.TREEITEMTYPE_PLCTEXTLIST,			// Text List for Visu
		/// <summary>
		/// Global Image Pool (627)
		/// </summary>
		PlcGlobalImagePool = TREEITEMTYPES.TREEITEMTYPE_PLCGLOBALIMAGEPOOL,	// Global Image Pool for Visu 
		/// <summary>
		/// Image Pool (628)
		/// </summary>
		PlcImagePool = TREEITEMTYPES.TREEITEMTYPE_PLCIMAGEPOOL,		// Image Pool for Visu
		/// <summary>
		/// Parameter List (629)
		/// </summary>
		PlcGvlParameters = TREEITEMTYPES.TREEITEMTYPE_PLCGVLPARAMLIST,		// Global ParameterList

		// Non codesys objects

		/// <summary>
		/// Plc Program Reference (650)
		/// </summary>
		PlcProgramReference = TREEITEMTYPES.TREEITEMTYPE_PLCPROGREF,
		/// <summary>
		/// Plc External Data Type (Defined in System Manager) (651)
		/// </summary>
		PlcExternalDataType = TREEITEMTYPES.TREEITEMTYPE_PLCEXTDATATYPE,
		/// <summary>
		/// Plc External Data Type Container (652)
		/// </summary>
		PlcExternalDataTypeContainer = TREEITEMTYPES.TREEITEMTYPE_PLCEXTDATATYPECONT,
		/// <summary>
		/// Plc Tmc Description File (653)
		/// </summary>
		PlcTmcDescription = TREEITEMTYPES.TREEITEMTYPE_PLCTMCDESCRIPTION,

		/// <summary>
		/// Plc interface property getter (654)
		/// </summary>
		PlcItfPropGet = TREEITEMTYPES.TREEITEMTYPE_PLCITFPROPGET,
		
		/// <summary>
		/// Plc Interface property setter (655)
		/// </summary>
		PlcItfPropSet = TREEITEMTYPES.TREEITEMTYPE_PLCITFPROPSET,

		/// <summary>
		/// Nested SAFETY Project Root (800)
		/// </summary>
		SafProjectDef = TREEITEMTYPES.TREEITEMTYPE_SAF_PROJECTDEF,

		/// <summary>
		/// "Safety Application (801)
		/// </summary>
		SafApplication = TREEITEMTYPES.TREEITEMTYPE_SAF_APP,
		/// <summary>
		/// "Safety Alias Device Folder (802)
		/// </summary>
		SafAliasDevices = TREEITEMTYPES.TREEITEMTYPE_SAF_ALIASDEV,
		/// <summary>
		/// "Safety Group Folder (803)
		/// </summary>
		SafGroup = TREEITEMTYPES.TREEITEMTYPE_SAF_GROUP,
		/// <summary>
		/// "Safety Generated Code Folder (804)
		/// </summary>
		SafGeneratedCode = TREEITEMTYPES.TREEITEMTYPE_SAF_GENERATEDCODE,
		/// <summary>
		/// "Safety Application Language File (805)
		/// </summary>
		SafSALFile = TREEITEMTYPES.TREEITEMTYPE_SAF_SAL_FILE,
		/// <summary>
		/// "Safety C Header File File (806)
		/// </summary>
		SafHFile = TREEITEMTYPES.TREEITEMTYPE_SAF_H_FILE,
		/// <summary>
		/// "Safety C Implementation File File (807)
		/// </summary>
		SafCFile = TREEITEMTYPES.TREEITEMTYPE_SAF_C_FILE,
		/// <summary>
		/// "Nested SAFETY Project Root (808)
		/// </summary>
		SafSDSFile = TREEITEMTYPES.TREEITEMTYPE_SAF_SDS_FILE,
		/// <summary>
		/// Safety target System config file (*.xml) (809))
		/// </summary>
		SafTargetConfigFile = TREEITEMTYPES.TREEITEMTYPE_SAF_SYSTEMCONFIG,
		/// <summary>
		/// Safety dependent file node (809))
		/// </summary>
		SafDependentFile = TREEITEMTYPES.TREEITEMTYPE_SAF_DEPENDENTFILE,

		// EndSection Safety

		// Section CPP Nested Projects
		/// <summary>
		/// "Nested CPP Project Root (900)
		/// </summary>
		Cpp_ProjectDef = TREEITEMTYPES.TREEITEMTYPE_CPP_PROJECTDEF	// Nested Project Root 
	// EndSection Safety

		// Non Codesys Objects
	}

	/// <summary>
	/// Box Type
	/// </summary>
	public enum BoxType
	{
		/// <summary>
		/// NotProcessed Box Type (BOXTYPE_UNKNOWN)
		/// </summary>
		[Description("NotProcessed Box Type (BOXTYPE_UNKNOWN)")]
		Unknown = 0,
		/// <summary>
		/// Lightbus-Buskoppler für bis zu 64 Busklemmen (BK2000) (BOXTYPE_BK2000)
		/// </summary>
		[Description("Lightbus-Buskoppler für bis zu 64 Busklemmen (BK2000) (BOXTYPE_BK2000)")]
		Lightbus_BK2000 = 1,
		/// <summary>
		/// Lightbus-Modul, 32 Bit Digital-Ein-/Ausgabemodul, 24 V DC (32Bit Box)(BOXTYPE_M1400)
		/// </summary>
		[Description("Lightbus-Modul, 32 Bit Digital-Ein-/Ausgabemodul, 24 V DC (32Bit Box)(BOXTYPE_M1400)")]
		Lightbus_M1400 = 2,
		/// <summary>
		/// Lightbus Modul, 4 Analog-Eingabe und 16 digitale E/A-Kanäle (M2400) (BOXTYPE_M2400)
		/// </summary>
		[Description("Lightbus Modul, 4 Analog-Eingabe und 16 digitale E/A-Kanäle (M2400) (BOXTYPE_M2400)")]
		Lightbus_M2400 = 3,
		/// <summary>
		/// Lightbus Modul, Mehrkanal-Inkremental-Encoder (M3xx0)(BOXTYPE_M3120_1)
		/// </summary>
		[Description("Lightbus Modul, Mehrkanal-Inkremental-Encoder (M3xx0)(BOXTYPE_M3120_1)")]
		Lightbus_M3xx0 = 4,
		/// <summary>
		/// Lightbus Modul, Mehrkanal-Inkremental-Encoder (M3120-2)(BOXTYPE_M3120_2)
		/// </summary>
		[Description("Lightbus Modul, Mehrkanal-Inkremental-Encoder (M3120-2)(BOXTYPE_M3120_2)")]
		Lightbus_M3120_2 = 5,
		/// <summary>
		/// Lightbus Modul, Mehrkanal-Inkremental-Encoder (M3120_3)(BOXTYPE_M3120_3)
		/// </summary>
		[Description("Lightbus Modul, Mehrkanal-Inkremental-Encoder (M3120_3)(BOXTYPE_M3120_3)")]
		Lightbus_M3120_3 = 6,
		/// <summary>
		/// Lightbus Modul, Mehrkanal-Inkremental-Encoder (M3120_4)(BOXTYPE_M3120_4)
		/// </summary>
		[Description("Lightbus Modul, Mehrkanal-Inkremental-Encoder (M3120_4)(BOXTYPE_M3120_4)")]
		Lightbus_M3120_4 = 7,
		/// <summary>
		/// Lightbus Modul, Absolut-Encoder (M3000) (BOXTYPE_M3000)
		/// </summary>
		[Description("Lightbus Modul, Absolut-Encoder (M3000) (BOXTYPE_M3000)")]
		Lightbus_M3000 = 8,
		/// <summary>
		/// C1120 Slave Module (in S5-Rack) (C1120-Slave) (BOXTYPE_C1120)
		/// </summary>
		[Description("C1120 Slave Module (in S5-Rack) (C1120-Slave) (BOXTYPE_C1120)")]
		Lightbus_C1120 = 9,
		/// <summary>
		/// Lightbus-Buskoppler für bis zu 64 digitale Busklemmen (BK2010)(BOXTYPE_BK2010)
		/// </summary>
		[Description("Lightbus-Buskoppler für bis zu 64 digitale Busklemmen (BK2010)(BOXTYPE_BK2010)")]
		Lightbus_BK2010 = 10,
		/// <summary>
		/// Antriebstechnik: Digital Kompakt Servoverstärker(BOXTYPE_AX2000_B200)
		/// </summary>
		[Description("Antriebstechnik: Digital Kompakt Servoverstärker(BOXTYPE_AX2000_B200)")]
		Lightbus_AX2xxx_B200 = 11,		// Seidel Antrieb
		/// <summary>
		/// Lightbus Modul, 4 Analog-Eingabemodul (M2510)(BOXTYPE_M2510)
		/// </summary>
		[Description("Lightbus Modul, 4 Analog-Eingabemodul (M2510)(BOXTYPE_M2510)")]
		Lightbus_M2510 = 12,
		/// <summary>
		/// Programable CDL (Lightbus) (Prog-CDL)(BOXTYPE_PROG_CDL)
		/// </summary>
		[Description("Programable CDL (Lightbus) (Prog-CDL)(BOXTYPE_PROG_CDL)")]
		Lightbus_ProgCDL = 13,
		/// <summary>
		/// Lightbus-"Economy plus"-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung) (BK2020)(BOXTYPE_BK2020)
		/// </summary>
		[Description("Lightbus-Economy plus-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung) (BK2020)(BOXTYPE_BK2020)")]
		Lightbus_BK2020 = 20,
		/// <summary>
		/// Lightbus-Busklemmen Controller (BC2000) (BOXTYPE_BC2000)
		/// </summary>
		[Description("Lightbus-Busklemmen Controller (BC2000) (BOXTYPE_BC2000)")]
		Lightbus_BC2000 = 21,
		/// <summary>
		/// Fox-Module FOX-20 (FOX20) (BOXTYPE_FOX20)
		/// </summary>
		[Description("Fox-Module FOX-20 (FOX20) (BOXTYPE_FOX20)")]
		Lightbus_Fox20 = 31,
		/// <summary>
		/// TR Fox 50 Modul (24 Bit Absolut (SSI)) (FOX50) (BOXTYPE_FOX50)
		/// </summary>
		[Description("TR Fox 50 Modul (24 Bit Absolut (SSI)) (FOX50) (BOXTYPE_FOX50)")]
		Lightbus_Fox50 = 32,
		/// <summary>
		/// Fox-Module FOX-RK001 (FOXRK001)(BOXTYPE_FOXRK001)
		/// </summary>
		[Description("Fox-Module FOX-RK001 (FOXRK001)(BOXTYPE_FOXRK001)")]
		Lightbus_FoxRK001 = 33,
		/// <summary>
		/// Fox-Module FOX-RK002 (FOXRK002)(BOXTYPE_FOXRK002)
		/// </summary>
		[Description("Fox-Module FOX-RK002 (FOXRK002)(BOXTYPE_FOXRK002)")]
		Lightbus_FOXRK002 = 34,
		/// <summary>
		/// CP10x1 (Folientasten 8 Kanal, LightBus) (CP10x1) (BOXTYPE_CP1001)
		/// </summary>
		[Description("CP10x1 (Folientasten 8 Kanal, LightBus) (CP10x1) (BOXTYPE_CP1001)")]
		Lightbus_CP10x1 = 35,
		/// <summary>
		/// IPxxxx-B200 (compact box, LightBus)(IPx-B200)(BOXTYPE_IPXB2)
		/// </summary>
		[Description("IPxxxx-B200 (compact box, LightBus)(IPx-B200)(BOXTYPE_IPXB2)")]
		Lightbus_IPx_B200 = 40,
		/// <summary>
		/// ILxxxx-B200 (coupler box, LightBus)(ILx-B200)(BOXTYPE_ILXB2)
		/// </summary>
		[Description("ILxxxx-B200 (coupler box, LightBus)(ILx-B200)(BOXTYPE_ILXB2)")]
		Lightbus_ILx_B200 = 41,
		/// <summary>
		/// ILxxxx-C200 (plc box, LightBus) (ILx-C200) (BOXTYPE_ILXC2)
		/// </summary>
		[Description("ILxxxx-C200 (plc box, LightBus) (ILx-C200) (BOXTYPE_ILXC2)")]
		Lightbus_ILx_C200 = 42,

		//TSMBOX_200			= 50, // RH, Not used in System Manager
		//BX2000				= 51, // RH, Not used in System Manager
		/// <summary>
		/// CX1500-B200 (CX1500-B200)(BOXTYPE_CX1500_B200)
		/// </summary>
		[Description("CX1500-B200 (CX1500-B200)(BOXTYPE_CX1500_B200)")]
		Lightbus_CX1500_B200 = 52,
		/// <summary>
		/// Profibus DP/FMS-Buskoppler für bis zu 64 Busklemmen, 1,5 MBaud (BK3000) (BOXTYPE_BK3000)
		/// </summary>
		[Description("Profibus DP/FMS-Buskoppler für bis zu 64 Busklemmen, 1,5 MBaud (BK3000) (BOXTYPE_BK3000)")]
		Profibus_BK3000 = 1001,
		/// <summary>
		/// Profibus DP/FMS-Buskoppler für bis zu 64 Busklemmen, 12 MBaud (BK3100) (BOXTYPE_BK3100)
		/// </summary>
		[Description("Profibus DP/FMS-Buskoppler für bis zu 64 Busklemmen, 12 MBaud (BK3100) (BOXTYPE_BK3100)")]
		Profibus_BK3100 = 1002,
		/// <summary>
		/// GSD Box (GSD Box)(BOXTYPE_PBDP_GSD)
		/// </summary>
		[Description("GSD Box (GSD Box)(BOXTYPE_PBDP_GSD)")]
		Profibus__GsdBox = 1003,
		/// <summary>
		/// Profibus DP-Buskoppler für bis zu 64 digitale Busklemmen, 1,5 MBaud (BK3010)(BOXTYPE_BK3010)
		/// </summary>
		[Description("Profibus DP-Buskoppler für bis zu 64 digitale Busklemmen, 1,5 MBaud (BK3010)(BOXTYPE_BK3010)")]
		Profibus_BK3010 = 1004,
		/// <summary>
		/// Profibus DP-Buskoppler für bis zu 64 digitale Busklemmen, 12 MBaud (BK3110)(BOXTYPE_BK3110)
		/// </summary>
		[Description("Profibus DP-Buskoppler für bis zu 64 digitale Busklemmen, 12 MBaud (BK3110)(BOXTYPE_BK3110)")]
		Profibus_BK3110 = 1005,
		/// <summary>
		/// Profibus DP-Buskoppler mit LWL-Anschluss, 1,5 MBaud (BK3500)(BOXTYPE_BK3500)
		/// </summary>
		[Description("Profibus DP-Buskoppler mit LWL-Anschluss, 1,5 MBaud (BK3500)(BOXTYPE_BK3500)")]
		Profibus_BK3500 = 1006,
		/// <summary>
		/// Profibus DP-"Low Cost"-Buskoppler für bis zu 64 digitale Busklemmen, 12 MBaud (LC3100) (BOXTYPE_LC3100)
		/// </summary>
		[Description("Profibus DP-Low Cost-Buskoppler für bis zu 64 digitale Busklemmen, 12 MBaud (LC3100) (BOXTYPE_LC3100)")]
		Profibus_LC3100 = 1007,
		/// <summary>
		/// ProfiDrive MC (ProfiDrive MC) (BOXTYPE_PBDP_DRIVE)
		/// </summary>
		[Description("ProfiDrive MC (ProfiDrive MC) (BOXTYPE_PBDP_DRIVE)")]
		Profibus_ProfidriveMC = 1008,

		//BK3020				= 1009,(BOXTYPE_BK3120)
		/// <summary>
		/// Profibus DP-"Economy Plus"-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung), 12 MBaud (BK3120)(BOXTYPE_BK3120)
		/// </summary>
		[Description("Profibus DP-Economy Plus-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung), 12 MBaud (BK3120)(BOXTYPE_BK3120)")]
		Profibus_BK3120 = 1010,
		/// <summary>
		/// Profibus DP-Busklemmen Controller (BC3100) (BOXTYPE_BC3100)
		/// </summary>
		[Description("Profibus DP-Busklemmen Controller (BC3100) (BOXTYPE_BC3100)")]
		Profibus_BC3100 = 1011,
		/// <summary>
		/// Profidrive MC (double) (ProfiDrive2 MC) (BOXTYPE_PBDP_DRIVE2)
		/// </summary>
		[Description("Profidrive MC (double) (ProfiDrive2 MC) (BOXTYPE_PBDP_DRIVE2)")]
		Profibus_ProfiDrive2MC = 1012,
		/// <summary>
		/// Profidrive MC (triple)(ProfiDrive3 MC)(BOXTYPE_PBDP_DRIVE3)
		/// </summary>
		[Description("Profidrive MC (triple)(ProfiDrive3 MC)(BOXTYPE_PBDP_DRIVE3)")]
		Profibus_ProfiDrive3MC = 1013,
		/// <summary>
		/// Profidrive MC (fourfold)(ProfiDrive4 MC)(BOXTYPE_PBDP_DRIVE4)
		/// </summary>
		[Description("Profidrive MC (fourfold)(ProfiDrive4 MC)(BOXTYPE_PBDP_DRIVE4)")]
		Profibus_ProfiDrive4MC = 1014,
		/// <summary>
		/// Profidrive MC (fivefold)(ProfiDrive5 MC)(BOXTYPE_PBDP_DRIVE5)
		/// </summary>
		[Description("Profidrive MC (fivefold)(ProfiDrive5 MC)(BOXTYPE_PBDP_DRIVE5)")]
		Profibus_ProfiDrive5MC = 1015,
		/// <summary>
		/// Profidrive MC (sixfold)(ProfiDrive6 MC)(BOXTYPE_PBDP_DRIVE6)
		/// </summary>
		[Description("Profidrive MC (sixfold)(ProfiDrive6 MC)(BOXTYPE_PBDP_DRIVE6)")]
		Profibus_ProfiDrive6MC = 1016,
		/// <summary>
		/// Profidrive MC (sevenfold)(ProfiDrive7 MC)(BOXTYPE_PBDP_DRIVE7)
		/// </summary>
		[Description(" Profidrive MC (sevenfold)(ProfiDrive7 MC)(BOXTYPE_PBDP_DRIVE7)")]
		Profibus_ProfiDrive7MC = 1017,
		/// <summary>
		/// Profidrive MC (eightfold)(ProfiDrive8 MC)(BOXTYPE_PBDP_DRIVE8)
		/// </summary>
		[Description("Profidrive MC (eightfold)(ProfiDrive8 MC)(BOXTYPE_PBDP_DRIVE8)")]
		Profibus_ProfiDrive8MC = 1018,
		/// <summary>
		/// Profibus DP-"Compact"-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung), 12 MBaud (BK3150)(BOXTYPE_BK3150)
		/// </summary>
		[Description("Profibus DP-Compact-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung), 12 MBaud (BK3150)(BOXTYPE_BK3150)")]
		Profibus_BK3150 = 1019,
		/// <summary>
		/// Profibus Slave BC3150 (BC3150) (BOXTYPE_BC3150)
		/// </summary>
		[Description("Profibus Slave BC3150 (BC3150) (BOXTYPE_BC3150)")]
		Profibus_BC3150 = 1020,
		//		/// <summary>
		//		/// BOXTYPE_BK3XXX(BOXTYPE_BK3XXX)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		BK3XXX				= 1021,
		//		/// <summary>
		//		/// BOXTYPE_BC3XXX(BOXTYPE_BC3XXX)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		BC3XXX				= 1022,
		/// <summary>
		/// IPxxxx-B3xx (compact box, Profibus)(IPx-B3xx)(BOXTYPE_IPXB3)
		/// </summary>
		[Description("Profibus Slave BC3150 (BC3150) (BOXTYPE_BC3150)")]
		Profibus_IPx_B3xx = 1030,
		/// <summary>
		/// ILxxxx-B3xx (coupler box, Profibus)(ILB3xx)(BOXTYPE_ILXB3)
		/// </summary>
		[Description("ILxxxx-B3xx (coupler box, Profibus)(ILB3xx)(BOXTYPE_ILXB3)")]
		Profibus_ILB3xx = 1031,
		/// <summary>
		/// ILxxxx-C3xx (plc box, Profibus)(ILC3xx)(BOXTYPE_ILXC3)
		/// </summary>
		[Description("ILxxxx-C3xx (plc box, Profibus)(ILC3xx)(BOXTYPE_ILXC3)")]
		Profibus_ILC3xx = 1032,
		/// <summary>
		/// TwinCAT Slave (Profibus)(TSMBOX_310)(BOXTYPE_TSMBOX_310)
		/// </summary>
		[Description("TwinCAT Slave (Profibus)(TSMBOX_310)(BOXTYPE_TSMBOX_310)")]
		Profibus_TsmBox_310 = 1040,
		/// <summary>
		/// Profibus DP-Busklemmen Controller(BX3100)(BOXTYPE_BX3100)
		/// </summary>
		[Description("Profibus DP-Busklemmen Controller(BX3100)(BOXTYPE_BX3100)")]
		Profibus_BX3100 = 1041,
		/// <summary>
		/// Profibus Slave CX1500-B310, PC104 (CX1500-B310)(BOXTYPE_CX1500_B310)
		/// </summary>
		[Description("Profibus Slave CX1500-B310, PC104 (CX1500-B310)(BOXTYPE_CX1500_B310)")]
		Profibus_CX1500_B310 = 1042,
		/// <summary>
		/// FC310x-Slave (FC310x-Slave)(BOXTYPE_FC310X_SLAVE)
		/// </summary>
		[Description("FC310x-Slave (FC310x-Slave)(BOXTYPE_FC310X_SLAVE)")]
		Profibus_FC310x_SLAVE = 1043,
		/// <summary>
		/// Antriebstechnik: Digital Kompakt Servoverstärker (Profibus) (AX2xxx-B310)(BOXTYPE_AX2000_B310)
		/// </summary>
		[Description("Antriebstechnik: Digital Kompakt Servoverstärker (Profibus) (AX2xxx-B310)(BOXTYPE_AX2000_B310)")]
		Profibus_AX2xxx_B310 = 1051,
		//		/// <summary>
		//		/// (BOXTYPE_TCPBDPSLAVE)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCPBDPSLAVE			= 1100,
		//		/// <summary>
		//		/// (BOXTYPE_TCFDLAGAG)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCFDLAGAG			= 1101,
		//		/// <summary>
		//		/// (BOXTYPE_TCMPI)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCMPI				= 1102,
		//		/// <summary>
		//		/// (BOXTYPE_TCPBMCSLAVE)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCPBMCSLAVE			= 1103,
		//		/// <summary>
		//		/// (BOXTYPE_TCPBMCSLAVE2)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCPBMCSLAVE2		= 1104,
		//		/// <summary>
		//		/// (BOXTYPE_TCPBMCSLAVE3)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCPBMCSLAVE3		= 1105,
		//		/// <summary>
		//		/// (BOXTYPE_TCPBMCSLAVE4)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCPBMCSLAVE4		= 1106,
		//		/// <summary>
		//		/// (BOXTYPE_TCPBMCSLAVE5)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCPBMCSLAVE5		= 1107,
		//		/// <summary>
		//		/// (BOXTYPE_TCPBMCSLAVE6)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCPBMCSLAVE6		= 1108,
		//		/// <summary>
		//		/// (BOXTYPE_TCPBMCSLAVE7)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCPBMCSLAVE7		= 1109,
		//		/// <summary>
		//		/// (BOXTYPE_TCPBMCSLAVE8)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCPBMCSLAVE8		= 1110,
		//		/// <summary>
		//		/// (BOXTYPE_TCPBMONSLAVE)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCPBMONSLAVE		= 1111,

		/// <summary>
		/// Interbus-Buskoppler für bis zu 64 Busklemmen (BK4000)(BOXTYPE_BK4000)
		/// </summary>
		[Description("Interbus-Buskoppler für bis zu 64 Busklemmen (BK4000)(BOXTYPE_BK4000)")]
		Interbus_BK4000 = 2001,
		/// <summary>
		/// IBS Box (IBS Box)(BOXTYPE_IBS_GENERIC)
		/// </summary>
		[Description("IBS Box (IBS Box)(BOXTYPE_IBS_GENERIC)")]
		Interbus_Generic = 2002,
		//		/// <summary>
		//		/// (BOXTYPE_IBS_BK)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		IBS_BK				= 2003,
		/// <summary>
		/// Interbus-Buskoppler für bis zu 64 digitale Busklemmen (BK4010)(BOXTYPE_BK4010)
		/// </summary>
		[Description("Interbus-Buskoppler für bis zu 64 digitale Busklemmen (BK4010)(BOXTYPE_BK4010)")]
		Interbus_BK4010 = 2004,
		/// <summary>
		/// Interbus-Buskoppler mit LWL-Anschluss für bis zu 64 Busklemmen (BK4500)(BOXTYPE_BK4500)
		/// </summary>
		[Description("Interbus-Buskoppler mit LWL-Anschluss für bis zu 64 Busklemmen (BK4500)(BOXTYPE_BK4500)")]
		Interbus_BK4500 = 2005,
		/// <summary>
		/// BK4510 (economy fieldbus coupler, InterBus-S Fiber)("BK4510)(BOXTYPE_BK4510)
		/// </summary>
		[Description("BK4510 (economy fieldbus coupler, InterBus-S Fiber)(BK4510)(BOXTYPE_BK4510)")]
		Interbus_BK4510 = 2006,
		//		/// <summary>
		//		/// IBS Slave(BOXTYPE_IBS_SLAVEBOX)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		IBS_SLAVEBOX		= 2007,
		/// <summary>
		/// Interbus-Busklemmen Controller (BC4000)(BOXTYPE_BC4000)
		/// </summary>
		[Description("Interbus-Busklemmen Controller (BC4000)(BOXTYPE_BC4000)")]
		Interbus_BC4000 = 2008,
		/// <summary>
		/// Interbus-"Economy plus"-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung)(BK4020)(BOXTYPE_BK4020)
		/// </summary>
		[Description("Interbus-Economy plus-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung)(BK4020)(BOXTYPE_BK4020)")]
		Interbus_BK4020 = 2009,
		/// <summary>
		/// IPxxxx-B400 (compact box, InterBus-S)(IPx-B400)(BOXTYPE_IPXB4)
		/// </summary>
		[Description("IPxxxx-B400 (compact box, InterBus-S)(IPx-B400)(BOXTYPE_IPXB4)")]
		Interbus_IPx_B400 = 2030,
		/// <summary>
		/// ILxxxx-B400 (coupler box, InterBus-S)(ILx-B400)(BOXTYPE_ILXB4)
		/// </summary>
		[Description("ILxxxx-B400 (coupler box, InterBus-S)(ILx-B400)(BOXTYPE_ILXB4)")]
		Interbus_ILx_B400 = 2031,
		/// <summary>
		/// ILxxxx-C400 (plc box, InterBus-S)(ILx-C400)(BOXTYPE_ILXC4)
		/// </summary>
		[Description("ILxxxx-C400 (plc box, InterBus-S)(ILx-C400)(BOXTYPE_ILXC4)")]
		Interbus_ILx_C400 = 2032,
		/// <summary>
		/// CP9020 ('fieldbus coupler', Control Panel)(CP9020)(BOXTYPE_CP2020)
		/// </summary>
		[Description("CP9020 ('fieldbus coupler', Control Panel)(CP9020)(BOXTYPE_CP2020)")]
		LocalKBus_CP9020 = 2020,
		/// <summary>
		/// Sercos Drive (Sercos Drive)(BOXTYPE_SERCOSAXIS)
		/// </summary>
		[Description("Sercos Drive (Sercos Drive)(BOXTYPE_SERCOSAXIS)")]
		Sercos_Drive = 3001,
		/// <summary>
		/// Antriebstechnik: Digital Kompakt Servoverstärker (SERCOS)(AX2xxx-B750)(BOXTYPE_AX2000_B750)
		/// </summary>
		[Description("Antriebstechnik: Digital Kompakt Servoverstärker (SERCOS)(AX2xxx-B750)(BOXTYPE_AX2000_B750)")]
		Sercos_AX2xxx_B750 = 3002,
		/// <summary>
		/// BK7500 (fieldbus coupler, SERCOS 2/4 MBaud)" (BK7500)(BOXTYPE_BK7500)
		/// </summary>
		[Description("BK7500 (fieldbus coupler, SERCOS 2/4 MBaud) (BK7500)(BOXTYPE_BK7500)")]
		Sercos_BK7500 = 3011,
		/// <summary>
		/// BK7510 (economy fieldbus coupler, SERCOS 2/4/8/16 MBaud)(BK7510)(BOXTYPE_BK7510)
		/// </summary>
		[Description("BK7510 (economy fieldbus coupler, SERCOS 2/4/8/16 MBaud)(BK7510)(BOXTYPE_BK7510)")]
		Sercos_BK7510 = 3012,
		/// <summary>
		/// BK7520 (economy plus fieldbus coupler, SERCOS 2/4/8/16 MBaud)(BK7520)(BOXTYPE_BK7520)
		/// </summary>
		[Description("BK7520 (economy plus fieldbus coupler, SERCOS 2/4/8/16 MBaud)(BK7520)(BOXTYPE_BK7520)")]
		Sercos_BK7520 = 3013,
		//		/// <summary>
		//		/// Sercos Box M(BOXTYPE_SERCOSMASTERBOX)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		SERCOSMASTERBOX		= 3021,
		//		/// <summary>
		//		/// Sercos Slave S(BOXTYPE_SERCOSSLAVEBOX)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		SERCOSSLAVEBOX		= 3031,
		/// <summary>
		/// BK8100 (fieldbus coupler, COM Port, RS232)(BK8100)(BOXTYPE_BK8100)
		/// </summary>
		[Description(" BK8100 (fieldbus coupler, COM Port, RS232)(BK8100)(BOXTYPE_BK8100)")]
		RS232_BK8100 = 4001,
		//		/// <summary>
		//		/// BK8110 (economy fieldbus coupler, COM Port, RS232)(BOXTYPE_BK8110)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		BK8110				= 4002,
		/// <summary>
		/// RS485-Buskoppler für bis zu 64 Busklemmen(BK8000)(BOXTYPE_BK8000)
		/// </summary>
		[Description("RS485-Buskoppler für bis zu 64 Busklemmen(BK8000)(BOXTYPE_BK8000)")]
		RS485_BK8000 = 4003,
		//		/// <summary>
		//		/// BK8010 (economy fieldbus coupler, COM Port, RS485)(BOXTYPE_BK8010)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		BK8010				= 4004,
		/// <summary>
		/// CP9040 ('fieldbus coupler', Control Panel)(CP9040)(BOXTYPE_CP9040)
		/// </summary>
		[Description("CP9040 ('fieldbus coupler', Control Panel)(CP9040)(BOXTYPE_CP9040)")]
		RS232_CP9040 = 4005,
		/// <summary>
		/// RS485-Busklemmen Controller (BC8000)(BOXTYPE_BC8000)
		/// </summary>
		[Description("RS485-Busklemmen Controller (BC8000)(BOXTYPE_BC8000)")]
		RS485_BC8000 = 4011,
		/// <summary>
		/// RS232-Busklemmen Controller (BC8100)(BOXTYPE_BC8100)
		/// </summary>
		[Description("RS232-Busklemmen Controller (BC8100)(BOXTYPE_BC8100)")]
		RS232_BC8100 = 4012,
		/// <summary>
		/// IPxxxx-B800 (compact box, COM Port, RS485)(IPx-B800)(BOXTYPE_IPXB80)
		/// </summary>
		[Description("IPxxxx-B800 (compact box, COM Port, RS485)(IPx-B800)(BOXTYPE_IPXB80)")]
		RS485_IPx_B800 = 4030,
		/// <summary>
		/// ILxxxx-B800 (coupler box, COM Port, RS485)(ILx-B800)(BOXTYPE_ILXB80)
		/// </summary>
		[Description("ILxxxx-B800 (coupler box, COM Port, RS485)(ILx-B800)(BOXTYPE_ILXB80)")]
		RS485_ILx_B800 = 4031,
		/// <summary>
		/// ILxxxx-C800 (plc box, COM Port, RS485)(ILx-C800)(BOXTYPE_ILXC80)
		/// </summary>
		[Description("ILxxxx-C800 (plc box, COM Port, RS485)(ILx-C800)(BOXTYPE_ILXC80)")]
		RS485_ILx_C800 = 4032,
		/// <summary>
		/// IPxxxx-B810 (compact box, COM Port, RS232)(IPx-B810)(BOXTYPE_IPXB81)
		/// </summary>
		[Description("IPxxxx-B810 (compact box, COM Port, RS232)(IPx-B810)(BOXTYPE_IPXB81)")]
		RS232_IPx_B810 = 4040,
		/// <summary>
		/// ILxxxx-B810 (coupler box, COM Port, RS232)(ILx-B810)(BOXTYPE_ILXB81)
		/// </summary>
		[Description("ILxxxx-B810 (coupler box, COM Port, RS232)(ILx-B810)(BOXTYPE_ILXB81)")]
		RS232_ILx_B810 = 4041,
		/// <summary>
		/// ILxxxx-C810 (plc box, COM Port, RS232)(ILx-C810)(BOXTYPE_ILXC81)
		/// </summary>
		[Description("ILxxxx-C810 (plc box, COM Port, RS232)(ILx-C810)(BOXTYPE_ILXC81)")]
		RS232_ILx_C810 = 4042,

		/// <summary>
		/// CANopen Buskoppler Lowcost (BK5100)(BOXTYPE_BK5100)
		/// </summary>
		[Description("CANopen Buskoppler Lowcost (BK5100)(BOXTYPE_BK5100)")]
		CANOpen_BK5100 = 5001,
		/// <summary>
		/// CAN CAL-Buskoppler für bis zu 64 Busklemmen (BK5110)(BOXTYPE_BK5110)
		/// </summary>
		[Description("CAN CAL-Buskoppler für bis zu 64 Busklemmen (BK5110)(BOXTYPE_BK5110)")]
		CANOpen_BK5110 = 5002,
		/// <summary>
		/// CANopen Node (CANopen Node)(BOXTYPE_CANNODE) (ItemType 5003)
		/// </summary>
		[Description("CANopen Node (CANopen Node)(BOXTYPE_CANNODE) (ItemType 5003)")]
		CANOpen_Node = 5003,
		/// <summary>
		/// CANopen-Buskoppler „Economy plus“ für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung (BK5120)(BOXTYPE_BK5120)
		/// </summary>
		[Description("CANopen-Buskoppler „Economy plus“ für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung (BK5120)(BOXTYPE_BK5120)")]
		CANOpen_BK5120 = 5004,
		/// <summary>
		/// CANopen-„Low Cost“-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung)(LC5100)(BOXTYPE_LC5100)
		/// </summary>
		[Description("CANopen-„Low Cost“-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung)(LC5100)(BOXTYPE_LC5100)")]
		CANOpen_LC5100 = 5005,
		/// <summary>
		/// CANopen Drive (CANopen Drive)(BOXTYPE_CANDRIVE)
		/// </summary>
		[Description("CANopen Drive (CANopen Drive)(BOXTYPE_CANDRIVE)")]
		CANOpen_Drive = 5006,
		/// <summary>
		/// Antriebstechnik: Digital Kompakt Servoverstärker (CANOpen) (AX2xxx-B510)(BOXTYPE_AX2000_B510)
		/// </summary>
		[Description("Antriebstechnik: Digital Kompakt Servoverstärker (CANOpen) (AX2xxx-B510)(BOXTYPE_AX2000_B510)")]
		CANOpen_AX2xxx_B510 = 5007,
		/// <summary>
		/// CANopen-„Compact“-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung)(BK5150)(BOXTYPE_BK5150)
		/// </summary>
		[Description("CANopen-„Compact“-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung)(BK5150)(BOXTYPE_BK5150)")]
		CANOpen_BK5150 = 5008,
		/// <summary>
		/// CANopen-Busklemmen Controller(BC5150)(BOXTYPE_BC5150)
		/// </summary>
		[Description("CANopen-Busklemmen Controller(BC5150)(BOXTYPE_BC5150)")]
		CANOpen_BC5150 = 5011,
		/// <summary>
		/// IPxxxx-B51x (compact box, CANopen)(IPx-B51x)(BOXTYPE_IPXB51)
		/// </summary>
		[Description("IPxxxx-B51x (compact box, CANopen)(IPx-B51x)(BOXTYPE_IPXB51)")]
		CANOpen_IPx_B51x = 5030,
		/// <summary>
		/// ILxxxx-B51x (coupler box, CANopen)(ILx-B51x)(BOXTYPE_ILXB51)
		/// </summary>
		[Description("ILxxxx-B51x (coupler box, CANopen)(ILx-B51x)(BOXTYPE_ILXB51)")]
		CANOpen_ILx_B51x = 5031,
		/// <summary>
		/// ILxxxx-C51x (plc box, CANopen)(ILx-C51x)(BOXTYPE_ILXC51)
		/// </summary>
		[Description("ILxxxx-C51x (plc box, CANopen)(ILx-C51x)(BOXTYPE_ILXC51)")]
		CANOpen_ILx_C51x = 5032,
		/// <summary>
		/// TwinCAT Slave (CANopen)(TSMBOX_510)(BOXTYPE_TSMBOX_510)
		/// </summary>
		[Description("TwinCAT Slave (CANopen)(TSMBOX_510)(BOXTYPE_TSMBOX_510)")]
		CANOpen_TsmBox_510 = 5040,
		/// <summary>
		/// BX5100 (CANopen Slave)(BX5100)(BOXTYPE_BX5100)
		/// </summary>
		[Description("BX5100 (CANopen Slave)(BX5100)(BOXTYPE_BX5100)")]
		CANOpen_BX5100 = 5041,
		/// <summary>
		/// CX1500-B510 (CX1500-B510)(BOXTYPE_CX1500_B510)
		/// </summary>
		[Description("CX1500-B510 (CX1500-B510)(BOXTYPE_CX1500_B510)")]
		CANOpen_CX1500_B510 = 5042,
		/// <summary>
		/// FC510x Slave (FC510x Slave)(BOXTYPE_FC510XSLV)
		/// </summary>
		[Description("FC510x Slave (FC510x Slave)(BOXTYPE_FC510XSLV)")]
		CANOpen_FC510xSlave = 5043,
		//		/// <summary>
		//		/// (BOXTYPE_TCCANSLAVE)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCCANSLAVE			= 5050,
		/// <summary>
		/// DeviceNet-Buskoppler für bis zu 64 Busklemmen (BK5200)(BOXTYPE_BK5200)
		/// </summary>
		[Description("DeviceNet-Buskoppler für bis zu 64 Busklemmen (BK5200)(BOXTYPE_BK5200)")]
		DeviceNet_BK5200 = 5201,
		/// <summary>
		/// DeviceNet-Buskoppler für bis zu 64 digitale Busklemmen (BK5210)(BOXTYPE_BK5210)
		/// </summary>
		[Description("DeviceNet-Buskoppler für bis zu 64 digitale Busklemmen (BK5210)(BOXTYPE_BK5210)")]
		DeviceNet_BK5210 = 5202,
		/// <summary>
		/// DeviceNet Node (DN Node) (BOXTYPE_DEVICENET)
		/// </summary>
		[Description("DeviceNet Node (DN Node) (BOXTYPE_DEVICENET)")]
		DeviceNet_Node = 5203,
		/// <summary>
		/// DeviceNet-„Compact“-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung) (BK5220)(BOXTYPE_BK5220)
		/// </summary>
		[Description("DeviceNet-„Compact“-Buskoppler für bis zu 64 Busklemmen (255 mit K-Bus-Verlängerung) (BK5220)(BOXTYPE_BK5220)")]
		DeviceNet_BK5220 = 5204,
		/// <summary>
		/// DeviceNet-„Low Cost“-Buskoppler für bis zu 64 digitale Busklemmen (255 mit K-Bus-Verlängerung) (LC5200)(BOXTYPE_LC5200)
		/// </summary>
		[Description("DeviceNet-„Low Cost“-Buskoppler für bis zu 64 digitale Busklemmen (255 mit K-Bus-Verlängerung) (LC5200)(BOXTYPE_LC5200)")]
		DeviceNet_LC5200 = 5205,
		//		/// <summary>
		//		/// (BOXTYPE_BK52XX)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		BK52XX				= 5211,
		//		/// <summary>
		//		/// DeviceNet-Busklemmen Controller(BOXTYPE_BC52XX)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		BC52XX				= 5212,
		/// <summary>
		/// IPxxxx-B52x (compact box, DeviceNet)(IPx-B52x)(BOXTYPE_IPXB52)
		/// </summary>
		[Description("IPxxxx-B52x (compact box, DeviceNet)(IPx-B52x)(BOXTYPE_IPXB52)")]
		DeviceNet_IPx_B52x = 5230,
		/// <summary>
		/// ILxxxx-B52x (coupler box, DeviceNet)(ILx-B52x)(BOXTYPE_ILXB52)
		/// </summary>
		[Description("ILxxxx-B52x (coupler box, DeviceNet)(ILx-B52x)(BOXTYPE_ILXB52)")]
		DeviceNet_ILx_B52x = 5231,
		/// <summary>
		/// ILxxxx-C52x (plc box, DeviceNet)(ILx-C52x)(BOXTYPE_ILXC52)
		/// </summary>
		[Description("ILxxxx-C52x (plc box, DeviceNet)(ILx-C52x)(BOXTYPE_ILXC52)")]
		DeviceNet_ILx_C52x = 5232,
		//		/// <summary>
		//		/// (BOXTYPE_TSMBOX_520)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TSMBOX_520			= 5240,
		/// <summary>
		/// DeviceNet-Busklemmen Controller(BX5200)(BOXTYPE_BX5200)
		/// </summary>
		[Description("DeviceNet-Busklemmen Controller(BX5200)(BOXTYPE_BX5200)")]
		DeviceNet_BX5200 = 5241,
		/// <summary>
		/// CX1500-B520(CX1500-B520)(BOXTYPE_CX1500_B520)
		/// </summary>
		[Description("CX1500-B520(CX1500-B520)(BOXTYPE_CX1500_B520)")]
		DeviceNet_CX1500_B520 = 5242,
		/// <summary>
		/// FC5201 Slave (FC5201 Slave) (BOXTYPE_FC520XSLV)
		/// </summary>
		[Description("FC5201 Slave (FC5201 Slave) (BOXTYPE_FC520XSLV)")]
		DeviceNet_FC5201Slave = 5243,
		//		/// <summary>
		//		/// (BOXTYPE_TCDNSLAVE)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		TCDNSLAVE			= 5250,

		/// <summary>
		/// Safety PLC EL6900
		/// </summary>
		[Description("Safety PLC EL6900")]
		EtherCAT_EL6900 = 6900,
		/// <summary>
		/// Safety Digital Inputs
		/// </summary>
		[Description("Safety Digital Inputs")]
		EtherCAT_EL1904 = 1904,
		/// <summary>
		/// Safety Digital Outputs
		/// </summary>
		[Description("Safety Digital Outputs")]
		EtherCAT_EL2904 = 2904,

		/// <summary>
		/// Ethernet TCP/IP Bus Coupler up to 64 Terminals (BOXTYPE_BK9000)
		/// </summary>
		[Description("Ethernet TCP/IP Bus Coupler up to 64 Terminals (BOXTYPE_BK9000)")]
		Ethernet_BK9000 = 9001,
		/// <summary>
		/// Ethernet-TCP/IP Bus Coupler up to 64 Terminals and integrated 2-Channel Switch (BK9100)
		/// </summary>
		[Description("Ethernet-TCP/IP Bus Coupler up to 64 Terminals and integrated 2-Channel Switch (BK9100)")]
		Ethernet_BK9100 = 9002,
		/// <summary>
		/// Ethernet TCP/IP "Compact" Bus Coupler BK9050
		/// </summary>
		[Description("Ethernet TCP/IP Compact Bus Coupler BK9050")]
		Ethernet_BK9050 = 9005,
		/// <summary>
		/// Ethernet TCP/IP-Busklemmen Controller(BOXTYPE_BC9000)
		/// </summary>
		[Description("Ethernet TCP/IP-Busklemmen Controller(BOXTYPE_BC9000)")]
		Ethernet_BC9000 = 9011,
		/// <summary>
		/// Ethernet TCP/IP-Busklemmen Controller(BOXTYPE_BC9000)
		/// </summary>
		[Description("Ethernet TCP/IP-Busklemmen Controller(BOXTYPE_BC9000)")]
		Ethernet_BC9100 = 9012,
		/// <summary>
		/// Ethernet TCP/IP-Busklemmen Controller(BOXTYPE_BC9000)
		/// </summary>
		[Description("Ethernet TCP/IP-Busklemmen Controller(BOXTYPE_BC9000)")]
		Ethernet_BX9000 = 9013,
		/// <summary>
		/// Ethernet TCP/IP-Busklemmen Controller(BOXTYPE_BC9000)
		/// </summary>
		[Description("Ethernet TCP/IP-Busklemmen Controller(BOXTYPE_BC9000)")]
		Ethernet_BX9000SLV = 9014,
		/// <summary>
		/// IPxxxx-B900 (compact box, Ethernet)(IPx-B900)(BOXTYPE_IPXB9)
		/// </summary>
		[Description("IPxxxx-B900 (compact box, Ethernet)(IPx-B900)(BOXTYPE_IPXB9)")]
		Ethernet_IPx_B900 = 9030,
		/// <summary>
		/// ILxxxx-B900 (coupler box, Ethernet)(ILx-B900)(BOXTYPE_ILXB9)
		/// </summary>
		[Description("ILxxxx-B900 (coupler box, Ethernet)(ILx-B900)(BOXTYPE_ILXB9)")]
		Ethernet_ILx_B900 = 9031,
		/// <summary>
		/// ILxxxx-C900 (plc box, Ethernet)(ILx-C900)(BOXTYPE_ILXC9)
		/// </summary>
		[Description("ILxxxx-C900 (plc box, Ethernet)(ILx-C900)(BOXTYPE_ILXC9)")]
		Ethernet_ILx_C900 = 9032,
		/// <summary>
		/// Remote TwinCAT Task (RemoteTask)(BOXTYPE_REMOTETASK)
		/// </summary>
		[Description("Remote TwinCAT Task (RemoteTask)(BOXTYPE_REMOTETASK)")]
		Ethernet_RemoteTask = 9041,
		/// <summary>
		/// Netzwerkvariable Publisher(Publisher)(BOXTYPE_NV_PUB)
		/// </summary>
		[Description("Netzwerkvariable Publisher(Publisher)(BOXTYPE_NV_PUB)")]
		Ethernet_Publisher = 9051,
		/// <summary>
		/// Netzwerkvariablen Subscriber(Subscriber)(BOXTYPE_NV_SUB)
		/// </summary>
		[Description("Netzwerkvariablen Subscriber(Subscriber)(BOXTYPE_NV_SUB)")]
		Ethernet_Subscriber = 9052,
		/// <summary>
		/// Antriebstechnik: Digital Kompakt Servoverstärker (Ethernet)(AX2xxx-B900)(BOXTYPE_AX2000_B900)
		/// </summary>
		[Description("Antriebstechnik: Digital Kompakt Servoverstärker (Ethernet)(AX2xxx-B900)(BOXTYPE_AX2000_B900)")]
		Ethernet_AX2xxx_B900 = 9061,
		/// <summary>
		/// EtherCAT Frame (EtherCAT Frame)(BOXTYPE_FLB_FRAME)
		/// </summary>
		[Description("therCAT Frame (EtherCAT Frame)(BOXTYPE_FLB_FRAME)")]
		Ethernet_EtherCATFrame = 9071,
		/// <summary>
		/// BK1120 (economy plus fieldbus coupler, EtherCAT)(BOXTYPE_BK1120)
		/// </summary>
		[Description("BK1120 (economy plus fieldbus coupler, EtherCAT)(BOXTYPE_BK1120)")]
		EtherCAT_BK1120 = 9081,
		//		/// <summary>
		//		/// Antriebstechnik: Digital Kompakt Servoverstärker (???)(BOXTYPE_AX2000_B100)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		AX2000_B100			= 9085,	
		/// <summary>
		/// EK1000 Ethernet Bridge (Ethernet/EtherCAT)(EK1000)(BOXTYPE_EK1000)
		/// </summary>
		[Description("EK1000 Ethernet Bridge (Ethernet/EtherCAT)(EK1000)(BOXTYPE_EK1000)")]
		Ethernet_EK1000 = 9091,
		/// <summary>
		/// EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EK1100)
		/// </summary>
		/// <remarks>Should not be inserted directly</remarks>
		[Description("EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EK1100)")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the Generic Type EtherCAT_EXXXXX instead")]
		EtherCAT_EK1100 = 9092,
		/// <summary>
		/// EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6731)
		/// </summary>
		[Description("EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6731)")]
		EtherCAT_EL6731 = 9093,
		/// <summary>
		/// EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6751)
		/// </summary>
		[Description("EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6751)")]
		EtherCAT_EL6751 = 9094,
		/// <summary>
		/// EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6752)
		/// </summary>
		[Description("EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6752)")]
		EtherCAT_EL6752 = 9095,
		/// <summary>
		/// EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6731SLV)
		/// </summary>
		[Description("EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6731SLV)")]
		EtherCAT_EL6731SLV = 9096,
		/// <summary>
		/// EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6751SLV)
		/// </summary>
		[Description("EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6751SLV)")]
		EtherCAT_EL6751SLV = 9097,
		/// <summary>
		/// EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6752SLV)
		/// </summary>
		[Description("EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EL6752SLV)")]
		EtherCAT_EL6752SLV = 9098,
		/// <summary>
		/// EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EXXXXX = 9099)
		/// </summary>
		[Description("EK1100 Ethernet Coupler (EtherCAT)(BOXTYPE_EXXXXX)")]
		EtherCAT_EXXXXX = 9099,

		/// <summary>
		/// BOXTYPE_EL6601 = 9100,
		/// </summary>
		[Description("BOXTYPE_EL6601 = 9100,")]
		EtherCAT_EL6601 = 9100,

		/// <summary>
		/// BOXTYPE_EL6001 = 9101,
		/// </summary>
		[Description("BOXTYPE_EL6001 = 9101,")]
		EtherCAT_EL6001 = 9101,

		/// <summary>
		/// BOXTYPE_EL69XX = 9102,
		/// </summary>
		[Description("BOXTYPE_EL69XX = 9102,")]
		EtherCAT_EL69XX = 9102,

		/// <summary>
		/// BOXTYPE_EL6021 = 9103
		/// </summary>
		[Description("BOXTYPE_EL6021 = 9103")]
		EtherCAT_EL6021 = 9103,

		/// <summary>
		/// BOXTYPE_EL6720 = 9104
		/// </summary>
		[Description("BOXTYPE_EL6720 = 9104")]
		EtherCAT_EL6720 = 9104,

		/// <summary>
		/// BOXTYPE_FSOESLAVE = 9105
		/// </summary>
		[Description("BOXTYPE_FSOESLAVE = 9105")]
		EtherCAT_FSOESLAVE = 9105,

		/// <summary>
		/// BOXTYPE_EL6631 = 9106
		/// </summary>
		[Description("BOXTYPE_EL6631 = 9106")]
		EtherCAT_EL6631 = 9106,

		/// <summary>
		/// BOXTYPE_EL6631SLV = 9107
		/// </summary>
		[Description("BOXTYPE_EL6631SLV = 9107")]
		EtherCAT_EL6631SLV = 9107,

		//BOXTYPE_PNIODEVICE = 9121,
		//BOXTYPE_PNIOTCDEVICE = 9122,
		//BOXTYPE_PNIODEVICEINTF = 9123,
		//BOXTYPE_PNIO_DRIVE = 9124,
		//BOXTYPE_PNIOBK9103 = 9125,
		//BOXTYPE_PNIOILB903 = 9126,
		//BOXTYPE_PNIOEL6631SLV = 9127,
		//BOXTYPE_PNIOEK9300 = 9128,
		//BOXTYPE_PNIOEK9300INTF = 9129,
		//BOXTYPE_PNIOEL6631 = 9130,

		//BOXTYPE_EIPSLAVEINTF = 9133,

		//BOXTYPE_PTPSLAVEINTF = 9143,

		//BOXTYPE_RAWUDPINTF = 9151,


		/// <summary>
		/// USB-Buskoppler für bis zu 64 Busklemmen(BK9500)(BOXTYPE_BK9500)
		/// </summary>
		[Description("USB-Buskoppler für bis zu 64 Busklemmen(BK9500)(BOXTYPE_BK9500)")]
		USB_BK9500 = 9500,
		//		/// <summary>
		//		/// (BOXTYPE_BK9510)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		BK9510				= 9510,
		//		/// <summary>
		//		/// (BOXTYPE_BK9520)
		//		/// </summary>
		//		        //		[DeviceManufacturer("Beckhoff")]
		//		BK9520				= 9520,
		/// <summary>
		/// Control Panel (USB)(CPX8XX)(BOXTYPE_CPX8XX)
		/// </summary>
		[Description("Control Panel (USB)(CPX8XX)(BOXTYPE_CPX8XX)")]
		USB_CPX8XX = 9591,
		/// <summary>
		/// CX1100 or CX9100 KBus extension('terminal coupler', CX1100-0002)(CX1100-BK)(BOXTYPE_CX1102) (SubType 9700)
		/// </summary>
		[Description("CX1100 or CX9100 KBus extension('terminal coupler', CX1100-0002)(CX1100-BK)(BOXTYPE_CX1102) (SubType 9700)")]
		KBus_CX1100_BK = 9700,
		/// <summary>
		/// CX9000 KBus extension('terminal coupler') (SubType: 9700)
		/// </summary>
		[Description("CX9000 KBus extension('terminal coupler') (SubType: 9700)")]
		KBus_CX9000_BK = 9700,
		/// <summary>
		/// CX1100 ('ip-link coupler', CX1100-0003)(CX1100-IP)(BOXTYPE_CX1103) (SubType 9701)
		/// </summary>
		[Description("CX1100 ('ip-link coupler', CX1100-0003)(CX1100-IP)(BOXTYPE_CX1103) (SubType 9701)")]
		KBus_CX1100_IP = 9701,
		/// <summary>
		/// CX1190 UPS ('uninterruptable power supplier')(CX1190-UPS)(BOXTYPE_CX1190) (Subtype 9702)
		/// </summary>
		[Description("CX1190 UPS ('uninterruptable power supplier')(CX1190-UPS)(BOXTYPE_CX1190) (Subtype 9702)")]
		KBus_CX1190 = 9702,
	}

	/// <summary>
	/// Type of the Device
	/// </summary>
	public enum DeviceType : int
	{
		/// <summary>
		/// 
		/// </summary>
		[Description("Unknown or uninitialized device type.")]
		Unknown = 0,
		/// <summary>
		/// Lightbus ISA interface card C1220 with communications processor (IODEVICETYPE_C1220, ID: 1)
		/// </summary>
		[Description("Lightbus ISA interface card C1220 with communications processor (IODEVICETYPE_C1220, ID: 1)")]
		Lightbus_C1220 = 1,
		/// <summary>
		/// Lightbus ISA interface card C1200 (IODEVICETYPE_C1200, ID: 2)
		/// </summary>
		[Description("Lightbus ISA interface card C1200 (IODEVICETYPE_C1200, ID: 2)")]
		Lightbus_C1200 = 2,
		/// <summary>
		/// ProfiBus Slave SPC3/IM182 (Siemens-Karte, IODEVICETYPE_SPC3, ID: 3)
		/// </summary>
		[Description("ProfiBus Slave SPC3/IM182 (Siemens, IODEVICETYPE_SPC3, ID: 3)")]
		Profibus_SPC3_IM182 = 3,
		/// <summary>
		/// ISA ProfiBus-Master CIF30 DPM(Hilscher-Karte, IODEVICETYPE_CIF30DPM, ID: 4)
		/// </summary>
		[Description("ISA ProfiBus-Master CIF30 DPM(Hilscher, IODEVICETYPE_CIF30DPM, ID: 4)")]
		Profibus_CIF30DPM = 4,
		/// <summary>
		/// ISA Interbus-S-Master CIF40 IBSM (Hilscher-Karte, IODEVICETYPE_CIF40IBSM, ID: 5)
		/// </summary>
		[Description("ISA Interbus-S-Master CIF40 IBSM (Hilscher-Karte, IODEVICETYPE_CIF40IBSM, ID: 5)")]
		Interbus_CIF40ISBM = 5,
		/// <summary>
		/// Beckhoff PC C2001 (IODEVICETYPE_BKHFPC, ID: 6)
		/// </summary>
		[Description("Beckhoff PC C2001 (IODEVICETYPE_BKHFPC, ID: 6)")]
		Beckhoff_C2001 = 6,
		/// <summary>
		/// ProfiBus-Master CP5412 (Siemens-Karte,IODEVICETYPE_CP5412A2, ID: 7) 
		/// </summary>
		[Description("ProfiBus-Master CP5412 (Siemens-Karte,IODEVICETYPE_CP5412A2, ID: 7) ")]
		Profibus_CP5412 = 7,
		/// <summary>
		/// Sercos Master SERCOS ISA (Indramat,IODEVICETYPE_SERCANSISA, ID: 8)
		/// </summary>
		[Description("Sercos Master SERCOS ISA (Indramat,IODEVICETYPE_SERCANSISA, ID: 8)")]
		Sercos_ISA = 8,
		/// <summary>
		/// Lpt Port (IODEVICETYPE_LPTPORT, ID: 9)
		/// </summary>
		[Description("Lpt Port (IODEVICETYPE_LPTPORT, ID: 9)")]
		Misc_LptPort = 9,
		/// <summary>
		/// Generic DPRAM NOV/DP-RAM (IODEVICETYPE_DPRAM, ID: 10)
		/// </summary>
		[Description("Generic DPRAM NOV/DP-RAM (IODEVICETYPE_DPRAM, ID: 10)")]
		Misc_NOV_DPRAM = 10,
		/// <summary>
		/// COM Port (IODEVICETYPE_COMPORT, ID: 11)
		/// </summary>
		[Description("COM Port (IODEVICETYPE_COMPORT, ID: 11)")]
		Misc_ComPort = 11,
		/// <summary>
		/// ISA CANopen-Master CIF30 CAN (Hilscher-Karte, IODEVICETYPE_CIF30CAN, ID:12)
		/// </summary>
		[Description("ISA CANopen-Master CIF30 CAN (Hilscher-Karte, IODEVICETYPE_CIF30CAN, ID:12)")]
		CanOpen_CIF30CAN = 12,
		/// <summary>
		/// ISA ProfiBus-Master CIF30 PB (Hilscher-Karte, IODEVICETYPE_CIF30PB, ID:13)
		/// </summary>
		[Description("ISA ProfiBus-Master CIF30 PB (Hilscher-Karte, IODEVICETYPE_CIF30PB, ID:13)")]
		Profibus_CIF30PB = 13,
		/// <summary>
		/// Beckhoff CP2030  (v1.0)(Beckhoff Panel Link (V1.0), IODEVICETYPE_BKHFCP2030, ID:14)
		/// </summary>
		[Description("Beckhoff CP2030  (v1.0)(Beckhoff Panel Link (V1.0), IODEVICETYPE_BKHFCP2030, ID:14)")]
		Beckhoff_CP2030 = 14,
		//		/// <summary>
		//		/// Interbus-S-Master (Phoenix-Karte, IODEVICETYPE_IBSSCIT)
		//		/// </summary>
		//		[DevGroup(DeviceGroup.Interbus)]
		//		IIBSSCIT			= 15,		 
		/// <summary>
		/// ISA Interbus-S-Master CIF30 IBM (Hilscher-Karte, IODEVICETYPE_CIF30IBM, ID:16)
		/// </summary>
		[Description("ISA Interbus-S-Master CIF30 IBM (Hilscher-Karte, IODEVICETYPE_CIF30IBM, ID:16)")]
		Interbus_CIF30IBM = 16,
		/// <summary>
		/// ISA DeviceNet-Master CIF30 DNM(Hilscher-Karte, IODEVICETYPE_CIF30DNM, ID:17)
		/// </summary>
		[Description("ISA DeviceNet-Master CIF30 DNM(Hilscher-Karte, IODEVICETYPE_CIF30DNM, ID:17)")]
		DeviceNet_CIF30DNM = 17,
		//		/// <summary>
		//		/// Beckhoff-Feldbuskarte (IODEVICETYPE_FCXXXX)
		//		/// </summary>
		//		[DevGroup(DeviceGroup.NotProcessed)]
		//		FCXXXX			= 18,		 
		/// <summary>
		/// PCI ProfiBus-Master CIF50 PB (Hilscher-Karte, IODEVICETYPE_CIF50PB, ID:19)
		/// </summary>
		[Description("PCI ProfiBus-Master CIF50 PB (Hilscher-Karte, IODEVICETYPE_CIF50PB, ID:19)")]
		Profibus_CIF50PB = 19,
		/// <summary>
		/// PCI Interbus-S-Master CIF50 IBM (Hilscher-Karte, IODEVICETYPE_CIF50IBM)
		/// </summary>
		[Description("PCI Interbus-S-Master CIF50 IBM (Hilscher-Karte, IODEVICETYPE_CIF50IBM)")]
		Interbus_CIF50IBM = 20,
		/// <summary>
		/// PCI DeviceNet-Master CIF50 DNM (Hilscher-Karte, IODEVICETYPE_CIF50DNM)
		/// </summary>
		[Description("PCI DeviceNet-Master CIF50 DNM (Hilscher-Karte, IODEVICETYPE_CIF50DNM)")]
		DeviceNet_CIF50DNM = 21,
		/// <summary>
		/// PCI CANopen-Master CIF50 CAN (Hilscher-Karte, IODEVICETYPE_CIF50CAN)
		/// </summary>
		[Description("PCI CANopen-Master CIF50 CAN (Hilscher-Karte, IODEVICETYPE_CIF50CAN)")]
		CANOpen_CIF50CAN = 22,
		/// <summary>
		/// PCMCIA ProfiBus-Master CIF60 PB (Hilscher-Karte, IODEVICETYPE_CIF60PB)
		/// </summary>
		[Description("PCMCIA ProfiBus-Master CIF60 PB (Hilscher-Karte, IODEVICETYPE_CIF60PB)")]
		Profibus_CIF60PB = 23,
		/// <summary>
		/// PCMCIA DeviceNet-Master CIF60 DNM(Hilscher-Karte, IODEVICETYPE_CIF60DNM)
		/// </summary>
		[Description("PCMCIA DeviceNet-Master CIF60 DNM(Hilscher-Karte, IODEVICETYPE_CIF60DNM)")]
		DeviceNet_CIF60DNM = 24,
		/// <summary>
		/// PCMCIA CANopen-Master CIF60 CAN(Hilscher-Karte, IODEVICETYPE_CIF60CAN)
		/// </summary>
		[Description("PCMCIA CANopen-Master CIF60 CAN(Hilscher-Karte, IODEVICETYPE_CIF60CAN)")]
		CANOpen_CIF60CAN = 25,
		/// <summary>
		/// PC104 ProfiBus-Master CIF104 DPM (Hilscher-Karte, IODEVICETYPE_CIF104DP)
		/// </summary>
		[Description("PC104 ProfiBus-Master CIF104 DPM (Hilscher-Karte, IODEVICETYPE_CIF104DP)")]
		Profibus_CIF104_DPM = 26,
		/// <summary>
		/// PC104 ProfiBus-Master CIF104 PB (Hilscher-Karte, IODEVICETYPE_C104PB)
		/// </summary>
		[Description("PC104 ProfiBus-Master CIF104 PB (Hilscher-Karte, IODEVICETYPE_C104PB)")]
		Profibus_CIF104PB = 27,
		/// <summary>
		/// PC104 Interbus-S-Master CIF104 IBM (Hilscher-Karte, IODEVICETYPE_C104IBM)
		/// </summary>
		[Description("PC104 Interbus-S-Master CIF104 IBM (Hilscher-Karte, IODEVICETYPE_C104IBM)")]
		Interbus_CIF104IBM = 28,
		/// <summary>
		/// PC104 CANopen-Master CIF104 CAN (Hilscher-Karte, IODEVICETYPE_C104CAN)
		/// </summary>
		[Description("PC104 CANopen-Master CIF104 CAN (Hilscher-Karte, IODEVICETYPE_C104CAN)")]
		CANOpen_C104CAN = 29,
		/// <summary>
		/// PC104 DeviceNet-Master CIF104 DNM (Hilscher-Karte, IODEVICETYPE_C104DNM)
		/// </summary>
		[Description("PC104 DeviceNet-Master CIF104 DNM (Hilscher-Karte, IODEVICETYPE_C104DNM)")]
		DeviceNet_CIF104DNM = 30,
		/// <summary>
		/// Beckhoff CP9030 Beckhoff Link (ISA, Panel-Link with UPS, IODEVICETYPE_BKHFCP9030)
		/// </summary>
		[Description("Beckhoff CP9030 Beckhoff Link (ISA, Panel-Link with UPS, IODEVICETYPE_BKHFCP9030)")]
		Beckhoff_CP9030 = 31,
		/// <summary>
		/// Motherboard System Management Bus SMB (IODEVICETYPE_SMB)
		/// </summary>
		[Description("Motherboard System Management Bus SMB (IODEVICETYPE_SMB)")]
		Misc_SystemManagementBus = 32,
		//		/// <summary>
		//		/// Beckhoff-PROFIBUS-Monitor (IODEVICETYPE_PBMON)
		//		/// </summary>
		//		[DevGroup(DeviceGroup.NotProcessed)]
		//		PBMON			= 33,		 
		/// <summary>
		/// PCI ProfiBus-Master CP5613 (Siemens-Karte, IODEVICETYPE_CP5613)
		/// </summary>
		[Description("PCI ProfiBus-Master CP5613 (Siemens-Karte, IODEVICETYPE_CP5613)")]
		Profibus_CP5613 = 34,
		/// <summary>
		/// PCMCIA Interbus-S-Master CIF60 IBM (Hilscher-Karte, IODEVICETYPE_CIF60IBM)
		/// </summary>
		[Description("PCMCIA Interbus-S-Master CIF60 IBM (Hilscher-Karte, IODEVICETYPE_CIF60IBM)")]
		Interbus_CIF60IBM = 35,
		/// <summary>
		/// Beckhoff-Lightbus-I/II-PCI-Karte FC200x (IODEVICETYPE_FC200X)
		/// </summary>
		[Description("Beckhoff-Lightbus-I/II-PCI-Karte FC200x (IODEVICETYPE_FC200X)")]
		Lightbus_FC200X = 36,
		// <summary>
		// nicht mehr benutzen (IODEVICETYPE_FC3100_OLD)
		// </summary>
		//FC3100_OLD		= 37,		 

		/// <summary>
		/// Beckhoff-Profibus-PCI-Karte FC310x (IODEVICETYPE_FC3100) (ItemType 38)
		/// </summary>
		[Description("Beckhoff-Profibus-PCI-Karte FC310x (IODEVICETYPE_FC3100) (ItemType 38)")]
		Profibus_FC310x = 38,
		/// <summary>
		/// Beckhoff-CanOpen-PCI-Karte FC510x (IODEVICETYPE_FC5100) (ItemType 39)
		/// </summary>
		[Description("Beckhoff-CanOpen-PCI-Karte FC510x (IODEVICETYPE_FC5100) (ItemType 39)")]
		CANOpen_FC510x = 39,
		/// <summary>
		/// Beckhoff-DeviceNet-PCI-Karte FC520x (IODEVICETYPE_FC5200) (ItemType 41)
		/// </summary>
		[Description("Beckhoff-DeviceNet-PCI-Karte FC520x (IODEVICETYPE_FC5200) (ItemType 41)")]
		DeviceNet_FC520x = 41,
		/// <summary>
		/// Beckhoff NC Rückwand Beckhoff NcBp (IODEVICETYPE_BKHFNCBP) (ItemType 43)
		/// </summary>
		[Description("Beckhoff NC Rückwand Beckhoff NcBp (IODEVICETYPE_BKHFNCBP) (ItemType 43)")]
		Beckhoff_NcBackPlane = 43,
		/// <summary>
		/// Sercos Master PCI SICAN/IAM PCI(, IODEVICETYPE_SERCANSPCI)
		/// </summary>
		[Description("Sercos Master PCI SICAN/IAM PCI(, IODEVICETYPE_SERCANSPCI)")]
		Sercos_PCIMaster = 44,
		/// <summary>
		/// Virtuelles Ethernet Device (IODEVICETYPE_ETHERNET)
		/// </summary>
		[Description("Virtuelles Ethernet Device (IODEVICETYPE_ETHERNET)")]
		Ethernet_VirtualEthernet = 45,
		/// <summary>
		/// Sercon 410B oder 816 Chip Master oder Slave (PCI) SERCONCHIP (IODEVICETYPE_SERCONPCI)
		/// </summary>
		[Description("ercon 410B oder 816 Chip Master oder Slave (PCI) SERCONCHIP (IODEVICETYPE_SERCONPCI)")]
		Sercos_SerconPCI = 46,
		//		/// <summary>
		//		/// Interbus-S-Master mit Slave-Teil auf LWL Basis (Phoenix-Karte, IODEVICETYPE_IBSSCRIRTLK)
		//		/// </summary>
		//		[DevGroup(DeviceGroup.Interbus)]
		//		Interbus_IBSSCRIRTLK	= 47,		 
		/// <summary>
		/// Beckhoff-SERCOS-PCI-Karte FC750x (IODEVICETYPE_FC7500)
		/// </summary>
		[Description("eckhoff-SERCOS-PCI-Karte FC750x (IODEVICETYPE_FC7500)")]
		Sercos_FC750x = 48,
		/// <summary>
		/// ISA Interbus-S-Slave (Hilscher-Karte,IODEVICETYPE_CIF30IBS)
		/// </summary>
		[Description("ISA Interbus-S-Slave (Hilscher-Karte,IODEVICETYPE_CIF30IBS)")]
		Interbus_CIF30IBS = 49,
		/// <summary>
		/// PCI Interbus-S-Slave CIF50 IBS (Hilscher-Karte, IODEVICETYPE_CIF50IBS)
		/// </summary>
		[Description("PCI Interbus-S-Slave CIF50 IBS (Hilscher-Karte, IODEVICETYPE_CIF50IBS)")]
		Interbus_CIF50IBS = 50,
		/// <summary>
		/// PC104 Interbus-S-Slave (Hilscher-Karte, IODEVICETYPE_C104IBS)
		/// </summary>
		[Description("PC104 Interbus-S-Slave (Hilscher-Karte, IODEVICETYPE_C104IBS)")]
		Interbus_CIF104IBS = 51,
		/// <summary>
		/// Beckhoff CP9040 Beckhoff CP PC (CP-PC, IODEVICETYPE_BKHFCP9040)
		/// </summary>
		[Description("Beckhoff CP9040 Beckhoff CP PC (CP-PC, IODEVICETYPE_BKHFCP9040)")]
		Beckhoff_CP9040 = 52,
		/// <summary>
		/// Beckhoff AH2000 (Hydraulik Backplane, IODEVICETYPE_BKHFAH2000, ID:53)
		/// </summary>
		[Description("Beckhoff AH2000 (Hydraulik Backplane, IODEVICETYPE_BKHFAH2000, ID:53)")]
		Beckhoff_AH2000 = 53,
		/// <summary>
		/// Beckhoff CP9035 (PCI, Panel-Link with UPS, IODEVICETYPE_BKHFCP9035, ID:54)
		/// </summary>
		[Description("Beckhoff CP9035 (PCI, Panel-Link with UPS, IODEVICETYPE_BKHFCP9035, ID:54)")]
		Beckhoff_CP9035 = 54,
		/// <summary>
		/// Beckhoff-AH2000 mit Profibus-MC (IODEVICETYPE_AH2000MC, ID:55)
		/// </summary>
		[Description("eckhoff-AH2000 mit Profibus-MC (IODEVICETYPE_AH2000MC, ID:55)")]
		Profibus_AH2000MC = 55,
		/// <summary>
		/// Beckhoff-Profibus-Monitor-PCI-Karte FC310x-Monitor (IODEVICETYPE_FC3100MON, ID:56)
		/// </summary>
		[Description("Beckhoff-Profibus-Monitor-PCI-Karte FC310x-Monitor (IODEVICETYPE_FC3100MON, ID:56)")]
		Profibus_FC310xMonitor = 56,
		/// <summary>
		/// Virtuelles USB Device (IODEVICETYPE_USB, ID:57)
		/// </summary>
		[Description("Virtuelles USB Device (IODEVICETYPE_USB, ID:57")]
		USB_Virtual = 57,
		/// <summary>
		/// Beckhoff-CANopen-Monitor-PCI-Karte FC510x-Monitor (IODEVICETYPE_FC5100MON, ID: 58)
		/// </summary>
		[Description("Beckhoff-CANopen-Monitor-PCI-Karte FC510x-Monitor (IODEVICETYPE_FC5100MON, ID: 58)")]
		CANOpen_FC510xMonitor = 58,
		/// <summary>
		/// Beckhoff-DeviceNet-Monitor-PCI-Karte FC520x-Monitor (IODEVICETYPE_FC5200MON)
		/// </summary>
		[Description("Beckhoff-DeviceNet-Monitor-PCI-Karte FC520x-Monitor (IODEVICETYPE_FC5200MON)")]
		DeviceNet_FC520xMonitor = 59,
		/// <summary>
		/// Beckhoff-Profibus-PCI-Karte als Slave FC310x-Slave (IODEVICETYPE_FC3100SLV)
		/// </summary>
		[Description("Beckhoff-Profibus-PCI-Karte als Slave FC310x-Slave (IODEVICETYPE_FC3100SLV)")]
		Profibus_FC310xSlave = 60,
		/// <summary>
		/// Beckhoff-CanOpen-PCI-Karte als Slave FC510x-Slave (IODEVICETYPE_FC5100SLV)
		/// </summary>
		[Description("Beckhoff-CanOpen-PCI-Karte als Slave FC510x-Slave (IODEVICETYPE_FC5100SLV)")]
		CANOpen_FC510xSlave = 61,
		/// <summary>
		/// Beckhoff-DeviceNet-PCI-Karte als Slave FC520x-Slave (IODEVICETYPE_FC5200SLV)
		/// </summary>
		[Description("Beckhoff-DeviceNet-PCI-Karte als Slave FC520x-Slave (IODEVICETYPE_FC5200SLV)")]
		DeviceNet_FC520xSlave = 62,
		/// <summary>
		/// PCI Interbus-S-Master IBS PCI SC/I-T (Phoenix-Karte, IODEVICETYPE_IBSSCITPCI)
		/// </summary>
		[Description("PCI Interbus-S-Master IBS PCI SC/I-T (Phoenix-Karte, IODEVICETYPE_IBSSCITPCI)")]
		Interbus_SCITPCI = 63,
		//		/// <summary>
		//		/// PCIInterbus-S-Master mit Slave-Teil auf LWL Basis (Phoenix-Karte, IODEVICETYPE_IBSSCRIRTLKPCI)
		//		/// </summary>
		//		[DevGroup(DeviceGroup.Interbus)]
		//		Interbus_SCRIRTLKPCI= 64,		 
		/// <summary>
		/// Beckhoff-CX1100 Klemmenbus Netzteil CX1100 (IODEVICETYPE_CX1100_BK, 65)
		/// </summary>
		[Description("Beckhoff-CX1100 Klemmenbus Netzteil CX1100 (IODEVICETYPE_CX1100_BK, 65)")]
		Beckhoff_CX1100 = 65,
		/// <summary>
		/// Ethernet Real Time Miniport RT-Ethernet (IODEVICETYPE_ENETRTMP, 66)
		/// </summary>
		[Description("Ethernet Real Time Miniport RT-Ethernet (IODEVICETYPE_ENETRTMP, 66)")]
		Ethernet_RTEthernet_TC2 = 66,
		/// <summary>
		/// 
		/// </summary>
		[Obsolete("Use only for TwinCAT 2.xx. For TC3 use EtherCAT_AutomationProtocol instead!", false)]
		Ethernet_RTEthernet = 66,

		/// <summary>
		/// PC104 Lightbus-Master CX1500-M200 (IODEVICETYPE_CX1500_M200, 67)
		/// </summary>
		[Description("PC104 Lightbus-Master CX1500-M200 (IODEVICETYPE_CX1500_M200, 67)")]
		Lightbus_CX1500_M200 = 67,
		/// <summary>
		/// PC104 Lightbus-Slave CX1500-B200 (IODEVICETYPE_CX1500_B200)
		/// </summary>
		[Description("PC104 Lightbus-Slave CX1500-B200 (IODEVICETYPE_CX1500_B200)")]
		Lightbus_CX1500_B200 = 68,
		/// <summary>
		/// PC104 ProfiBus-Master CX1500-M310 (IODEVICETYPE_CX1500_M310)
		/// </summary>
		[Description("PC104 ProfiBus-Master CX1500-M310 (IODEVICETYPE_CX1500_M310)")]
		Profibus_CX1500_M310 = 69,
		/// <summary>
		/// PC104 ProfiBus-Slave CX1500-B310 (IODEVICETYPE_CX1500_B310)
		/// </summary>
		[Description("PC104 ProfiBus-Slave CX1500-B310 (IODEVICETYPE_CX1500_B310)")]
		Profibus_CX1500_B310 = 70,
		/// <summary>
		/// PC104 CANopen-Master CX1500-M510 (IODEVICETYPE_CX1500_M510)
		/// </summary>
		[Description("PC104 CANopen-Master CX1500-M510 (IODEVICETYPE_CX1500_M510)")]
		CANOpen_CX1500_M510 = 71,
		/// <summary>
		/// PC104 CANopen-Slave CX1500-B510 (IODEVICETYPE_CX1500_B510)
		/// </summary>
		[Description("PC104 CANopen-Slave CX1500-B510 (IODEVICETYPE_CX1500_B510)")]
		CANOpen_CX1500_B510 = 72,
		/// <summary>
		/// PC104 DeviceNet-Master CX1500-M520 (IODEVICETYPE_CX1500_M520)
		/// </summary>
		[Description("PC104 DeviceNet-Master CX1500-M520 (IODEVICETYPE_CX1500_M520)")]
		DeviceNet_CX1500_M520 = 73,
		/// <summary>
		/// PC104 DeviceNet-Slave CX1500-B520 (IODEVICETYPE_CX1500_B520)
		/// </summary>
		[Description("PC104 DeviceNet-Slave CX1500-B520 (IODEVICETYPE_CX1500_B520)")]
		DeviceNet_CX1500_B520 = 74,
		/// <summary>
		/// PC104 Sercos-Master CX1500-M750 (IODEVICETYPE_CX1500_M750)
		/// </summary>
		[Description("PC104 Sercos-Master CX1500-M750 (IODEVICETYPE_CX1500_M750)")]
		Sercos_CX1500_M750 = 75,
		/// <summary>
		/// PC104 Sercos-Slave (IODEVICETYPE_CX1500_B750)
		/// </summary>
		[Description("PC104 Sercos-Slave (IODEVICETYPE_CX1500_B750)")]
		Sercos_CX1500_B750 = 76,
		/// <summary>
		/// BX Klemmenbus Interface BX-BK (IODEVICETYPE_BX_BK)
		/// </summary>
		[Description("BX Klemmenbus Interface BX-BK (IODEVICETYPE_BX_BK)")]
		Beckhoff_BX_BK = 77,
		/// <summary>
		/// BX SSB-Master BX-M510(IODEVICETYPE_BX_M510)
		/// </summary>
		[Description("BX SSB-Master BX-M510(IODEVICETYPE_BX_M510)")]
		CANOpen_BX_M510 = 78,
		/// <summary>
		/// BX ProfiBus-Slave BX-B310 (IODEVICETYPE_BX_B310)
		/// </summary>
		[Description("BX ProfiBus-Slave BX-B310 (IODEVICETYPE_BX_B310)")]
		Profibus_BX_B310 = 79,
		/// <summary>
		/// PCIInterbus-S-Master mit Slave-Teil auf Kupfer Basis IBS PCI SC/RI/I-T (Phoenix-Karte, IODEVICETYPE_IBSSCRIRTPCI)
		/// </summary>
		[Description("PCIInterbus-S-Master mit Slave-Teil auf Kupfer Basis IBS PCI SC/RI/I-T (Phoenix-Karte, IODEVICETYPE_IBSSCRIRTPCI)")]
		Interbus_SCRIRTPCI = 80,
		/// <summary>
		/// BX CANopen-Slave BX-B510 (IODEVICETYPE_BX_B510)
		/// </summary>
		[Description("BX CANopen-Slave BX-B510 (IODEVICETYPE_BX_B510)")]
		CANOpen_BX_B510 = 81,
		/// <summary>
		/// BX DeviceNet-Slave BX-B520 (IODEVICETYPE_BX_B520)
		/// </summary>
		[Description("BX DeviceNet-Slave BX-B520 (IODEVICETYPE_BX_B520)")]
		DeviceNet_BX_B520 = 82,
		/// <summary>
		/// BCxx50 ProfiBus-Slave BC3150 (IODEVICETYPE_BC3150)
		/// </summary>
		[Description("BCxx50 ProfiBus-Slave BC3150 (IODEVICETYPE_BC3150)")]
		Profibus_BC3150 = 83,
		/// <summary>
		/// BCxx50 CANopen-Slave (IODEVICETYPE_BC5150)
		/// </summary>
		[Description("BCxx50 CANopen-Slave (IODEVICETYPE_BC5150)")]
		CANOpen_BC5150 = 84,
		/// <summary>
		/// BCxx50 DeviceNet-Slave BC5250 (IODEVICETYPE_BC5250)
		/// </summary>
		[Description("BCxx50 DeviceNet-Slave BC5250 (IODEVICETYPE_BC5250)")]
		DeviceNet_BC5250 = 85,

		/// <summary>
		/// Beckhoff-Profibus-EtherCAT-Klemme (IODEVICETYPE_EL6731)
		/// </summary>
		[Description("Beckhoff - Profibus - EtherCAT - Klemme(IODEVICETYPE_EL6731))")]
		EtherCAT_EL6731 = 86,		// Beckhoff-Profibus-EtherCAT-Klemme
		/// <summary>
		/// Beckhoff-CanOpen-EtherCAT-Klemme (IODEVICETYPE_EL6751)
		/// </summary>
		[Description("Beckhoff-CanOpen-EtherCAT-Klemme (IODEVICETYPE_EL6751)")]
		EtherCAT_EL6751 = 87,		// Beckhoff-CanOpen-EtherCAT-Klemme
		/// <summary>
		/// Beckhoff-DeviceNet-EtherCAT-Klemme (IODEVICETYPE_EL6752)
		/// </summary>
		[Description("Beckhoff-DeviceNet-EtherCAT-Klemme (IODEVICETYPE_EL6752)")]
		EtherCAT_EL6752 = 88,		// Beckhoff-DeviceNet-EtherCAT-Klemme
		/// <summary>
		/// COM ProfiBus-Master 8 kByte (Hilscher-Karte)(IODEVICETYPE_COMPB)
		/// </summary>
		[Description("COM ProfiBus-Master 8 kByte (Hilscher-Karte)(IODEVICETYPE_COMPB)")]
		Profibus_COMPB = 89,		// COM ProfiBus-Master 8 kByte (Hilscher-Karte)
		/// <summary>
		/// COM Interbus-S-Master (Hilscher-Karte)(IODEVICETYPE_COMIBM)
		/// </summary>
		[Description("COM Interbus-S-Master (Hilscher-Karte)(IODEVICETYPE_COMIBM)")]
		Interbus_COMIBM = 90,		// COM Interbus-S-Master (Hilscher-Karte)
		/// <summary>
		/// COM DeviceNet-Master (Hilscher-Karte)(IODEVICETYPE_COMDNM)
		/// </summary>
		[Description("COM DeviceNet-Master (Hilscher-Karte)(IODEVICETYPE_COMDNM)")]
		DeviceNet_COMDNM = 91,		// COM DeviceNet-Master (Hilscher-Karte)
		/// <summary>
		/// COM CANopen-Master (Hilscher-Karte)(IODEVICETYPE_COMCAN)
		/// </summary>
		[Description("COM CANopen-Master (Hilscher-Karte)(IODEVICETYPE_COMCAN)")]
		CANOpen_COMCAN = 92,		// COM CANopen-Master (Hilscher-Karte)
		/// <summary>
		/// COM CANopen-Slave (Hilscher-Karte)(IODEVICETYPE_COMIBS)
		/// </summary>
		[Description("COM CANopen-Slave (Hilscher-Karte)(IODEVICETYPE_COMIBS)")]
		CANOpen_COMIBS = 93,		// COM CANopen-Slave (Hilscher-Karte)
		/// <summary>
		/// EtherCAT in direct mode (v2.10 only) (IODEVICETYPE_ETHERCAT)
		/// </summary>
		[Description("EtherCAT in direct mode (v2.10 only) (IODEVICETYPE_ETHERCAT)")]
		EtherCAT_DirectModeV210 = 94,		// EtherCAT in direct mode (V2.10 Version)
		/// <summary>
		/// PROFINET Master (IODEVICETYPE_PROFINETIOCONTROLLER)
		/// </summary>
		[Description("PROFINET Master (IODEVICETYPE_PROFINETIOCONTROLLER)")]
		Profinet_IOCONTROLLER = 95,	// PROFINET Master
		/// <summary>
		/// PROFINET Slave (IODEVICETYPE_PROFINETIODEVICE)
		/// </summary>
		[Description("PROFINET Slave (IODEVICETYPE_PROFINETIODEVICE)")]
		Profinet_IODEVICE = 96,		// PROFINET Slave
		/// <summary>
		/// Beckhoff-Profibus-Slave-EtherCAT-Klemme (IODEVICETYPE_EL6731SLV)
		/// </summary>
		[Description("Beckhoff-Profibus-Slave-EtherCAT-Klemme (IODEVICETYPE_EL6731SLV)")]
		Profibus_EL6731SLV = 97,		// Beckhoff-Profibus-Slave-EtherCAT-Klemme
		/// <summary>
		/// Beckhoff-CanOpen-Slave-EtherCAT-Klemme (IODEVICETYPE_EL6751SLV)
		/// </summary>
		[Description("Beckhoff-CanOpen-Slave-EtherCAT-Klemme (IODEVICETYPE_EL6751SLV)")]
		CANOpen_EL6751SLV = 98,		// Beckhoff-CanOpen-Slave-EtherCAT-Klemme
		/// <summary>
		/// Beckhoff-DeviceNet-Slave-EtherCAT-Klemme (IODEVICETYPE_EL6752SLV)
		/// </summary>
		[Description("Beckhoff-DeviceNet-Slave-EtherCAT-Klemme (IODEVICETYPE_EL6752SLV)")]
		DeviceNet_EL6752SLV = 99,		// Beckhoff-DeviceNet-Slave-EtherCAT-Klemme
		/// <summary>
		/// PC104+ ProfiBus-Master 8 kByte (Hilscher-Karte) (IODEVICETYPE_C104PPB)
		/// </summary>
		[Description("PC104+ ProfiBus-Master 8 kByte (Hilscher-Karte) (IODEVICETYPE_C104PPB)")]
		Profibus_C104PPB = 100,	// PC104+ ProfiBus-Master 8 kByte (Hilscher-Karte)
		/// <summary>
		/// PC104+ CANopen-Master (Hilscher-Karte) (IODEVICETYPE_C104PCAN)
		/// </summary>
		[Description("PC104+ CANopen-Master (Hilscher-Karte) (IODEVICETYPE_C104PCAN)")]
		CANOpen_C104PCAN = 101,	// PC104+ CANopen-Master (Hilscher-Karte)
		/// <summary>
		/// PC104+ DeviceNet-Master (Hilscher-Karte) (IODEVICETYPE_C104PDNM)
		/// </summary>
		[Description("PC104+ DeviceNet-Master (Hilscher-Karte) (IODEVICETYPE_C104PDNM)")]
		DeviceNet_C104PDNM = 102,	// PC104+ DeviceNet-Master (Hilscher-Karte)
		/// <summary>
		/// BCxx50 serieller Slave (IODEVICETYPE_BC8150)
		/// </summary>
		[Description("BCxx50 serieller Slave (IODEVICETYPE_BC8150)")]
		Serial_BC8150 = 103,	// BCxx50 serieller Slave
		/// <summary>
		/// BX9000 Ethernet Slave (IODEVICETYPE_BX9000)
		/// </summary>
		[Description("BX9000 Ethernet Slave (IODEVICETYPE_BX9000)")]
		Ethernet_BX9000 = 104,	// BX9000 Ethernet Slave
		/// <summary>
		/// CX9000 Terminal Device (K-BUS)
		/// </summary>
		[Description("CX9000 Terminal Device (K-BUS)")]
		Ethernet_CX9000 = 105,	// BX9000 Ethernet Slave
		/// <summary>
		/// EtherCAT Automation Protocol, EL6601 (IODEVICETYPE_EL6601 = 106)
		/// </summary>
		[Description("EtherCAT Automation Protocol, EL6601 (IODEVICETYPE_EL6601 = 106)")]
		RTEthernet_EL6601 = 106,	// Beckhoff-RT-Ethernet-EtherCAT-Klemme
		/// <summary>
		/// BC9050 Etherent Slave (IODEVICETYPE_BC9050 = 107)
		/// </summary>
		[Description("BC9050 Etherent Slave (IODEVICETYPE_BC9050 = 107)")]
		Ethernet_BC9050 = 107,	// BC9050 Etherent Slave
		/// <summary>
		/// RT-Ethernet Adapter (BC9120 Ethernet Slave) IODEVICETYPE_BC9120 = 108)
		/// </summary>
		[Description("RT-Ethernet Adapter (BC9120 Ethernet Slave) IODEVICETYPE_BC9120 = 108)")]
		Ethernet_BC9120 = 108,	// BC9120 Ethernet Slave
		/// <summary>
		/// RT-Ethernet Multiple Protocol Handler,  Ethernet Miniport Adapter (IODEVICETYPE_ENETADAPTER = 109)
		/// </summary>
		[Description("Ethernet Miniport Adapter (IODEVICETYPE_ENETADAPTER = 109)")]
		Ethernet_RTMultipleProtocol = 109,	// Ethernet Miniport Adapter
		/// <summary>
		/// 
		/// </summary>
		[Obsolete("Use Ethernet_RTMultipleProtocol instead!", false)]
		Ethernet_MiniportAdapter = 109,	// Ethernet Miniport Adapter
		/// <summary>
		/// BC9020 Ethernet Slave (IODEVICETYPE_BC9020 = 110)
		/// </summary>
		[Description("BC9020 Ethernet Slave (IODEVICETYPE_BC9020 = 110)")]
		Ethernet_BC9020 = 110,	// BC9020 Ethernet Slave
		/// <summary>
		/// EtherCAT Protocol in Direct mode (IODEVICETYPE_ETHERCATPROT = 111)
		/// </summary>
		[Description("EtherCAT Protocol in Direct mode (IODEVICETYPE_ETHERCATPROT = 111)")]
		EtherCAT_DirectMode = 111,	// EtherCAT Protocol in direct mode
		/// <summary>
		/// EtherCAT Network Variables (Automation protocol, IODEVICETYPE_ETHERNETNVPROT = 112)
		/// </summary>
		[Description("EtherCAT Network Variables (Automation protocol, IODEVICETYPE_ETHERNETNVPROT = 112)")]
		EtherCAT_AutomationProtocol = 112,	// 
		/// <summary>
		/// 
		/// </summary>
		[Obsolete("Renamed to ETherCAT_AutomationProtocol", false)]
		Ethernet_NetworkVariables = 112,	// 

		/// <summary>
		/// Profinet Controller (IODEVICETYPE_ETHERNETPNMPROT = 113)
		/// </summary>
		[Description("Profinet Controller (IODEVICETYPE_ETHERNETPNMPROT = 113)")]
		Profinet_Controller = 113,	// 
		/// <summary>
		///  Beckhoff-Lightbus-EtherCAT-Klemme (IODEVICETYPE_EL6720 = 114)
		/// </summary>
		[Description("Beckhoff-Lightbus-EtherCAT-Klemme (IODEVICETYPE_EL6720 = 114)")]
		Lightbus_EL6720 = 114,	// Beckhoff-Lightbus-EtherCAT-Klemme
		/// <summary>
		/// Profinet Device (IODEVICETYPE_ETHERNETPNSPROT = 115)
		/// </summary>
		[Description("Profinet Device (IODEVICETYPE_ETHERNETPNSPROT = 115)")]
		Profinet_Device = 115,	// 
		/// <summary>
		/// Beckhoff CP PC (Beckhoff CP6608(IXP PC), IODEVICETYPE_BKHFCP6608 = 116)
		/// </summary>
		[Description("Beckhoff CP PC (Beckhoff CP6608(IXP PC), IODEVICETYPE_BKHFCP6608 = 116)")]
		Beckhoff_CP6608 = 116,	//	Beckhoff CP6608(IXP PC)
		/// <summary>
		/// IEEE 1588 (PTP) (IODEVICETYPE_PTP_IEEE1588 = 117)
		/// </summary>
		[Description("IEEE 1588 (PTP) (IODEVICETYPE_PTP_IEEE1588 = 117)")]
		Ethernet_PTP_IEEE1588 = 117,
		/// <summary>
		/// EL6631-0010 (IODEVICETYPE_EL6631SLV = 118)
		/// </summary>
		[Description("EL6631-0010 (IODEVICETYPE_EL6631SLV = 118)")]
		Profinet_EL6631SLV = 118,	// EL6631-0010
		/// <summary>
		/// EL6631 (IODEVICETYPE_EL6631 = 119)
		/// </summary>
		[Description("EL6631 (IODEVICETYPE_EL6631 = 119)")]
		Profinet_EL6631 = 119,	// EL6631
		/// <summary>
		/// CX5000-BK (Beckhoff-CX5100 Klemmenbus Netzteil, IODEVICETYPE_CX5000_BK = 120)
		/// </summary>
		[Description("CX5000-BK (Beckhoff-CX5100 Klemmenbus Netzteil, IODEVICETYPE_CX5000_BK = 120)")]
		Beckhoff_CX5000_BK = 120,	// Beckhoff-CX5100 Klemmenbus Netzteil
		/// <summary>
		/// PCI DP-RAM (Generic PCI DPRAM (TCOM), IODEVICETYPE_PCIDEVICE = 121)
		/// </summary>
		[Description("PCI DP-RAM (Generic PCI DPRAM (TCOM), IODEVICETYPE_PCIDEVICE = 121)")]
		Misc_PciDevice = 121,	// Generic PCI DPRAM (TCOM)

		/// <summary>
		/// IODEVICETYPE_ETHERNETEAPPOLL	= 122,	// EtherCAT Automation Protocoll polled connection
		/// </summary>
		Ethernet_AutomationProtocolPolled = 122,
		/// <summary>
		/// IODEVICETYPE_ETHERNETAUTOPROT = 123,	// Automation Protocol
		/// </summary>
		Ethernet_AutomationProtocol = 123,
		/// <summary>
		/// IODEVICETYPE_CCAT					= 124,	// CCAT
		/// </summary>
		CCAT = 124,
		/// <summary>
		/// IODEVICETYPE_CPLINK3				= 125,	// Virtuelles USB Device (remote via CPLINK3)
		/// </summary>
		CPLink3_VirtualUSB = 125,
		/// <summary>
		/// IODEVICETYPE_EL6632				= 126,	// EL6632
		/// </summary>
		EL6632 = 123,
		/// <summary>
		/// IODEVICETYPE_CCAT_PBM			= 127,	// CCAT Profibus Master
		/// </summary>
		Profibus_CCATMaster = 127,
		/// <summary>
		/// IODEVICETYPE_CCAT_PBS			= 128,	// CCAT Profibus Slave
		/// </summary>
		Profibus_CCATSlave = 128,
		/// <summary>
		/// IODEVICETYPE_CCAT_CNM			= 129,	// CCAT CANopen Master
		/// </summary>
		CANOpen_CCATMaster = 129,
		/// <summary>
		/// IODEVICETYPE_ETHERCATSLAVE		= 130,	// EtherCAT Slave
		/// </summary>
		EtherCAT_Slave = 130,
		/// <summary>
		/// IODEVICETYPE_BACNET				= 131,	// BACnet device
		/// </summary>
		BACnet_Device = 131,
		/// <summary>
		/// IODEVICETYPE_CCAT_CNS			= 132,	// CCAT CANopen Slave
		/// </summary>
		CANOpen_CCATSlave = 132,
		/// <summary>
		/// IODEVICETYPE_ETHIP_SCANNER		= 133,  // ETHERNET IP  Master
		/// </summary>
		EThernetIP_Master = 133,
		/// <summary>
		/// IODEVICETYPE_ETHIP_ADAPTER		= 134,  // ETHERNET IP  Slave
		/// </summary>
		EthernetIP_Slave = 134,
		/// <summary>
		/// IODEVICETYPE_CX8000_BK			= 135,	// Beckhoff-CX8100 Klemmenbus Netzteil
		/// </summary>
		CX8000_BK = 135,
		/// <summary>
		/// IODEVICETYPE_ETHERNETUDPPROT	= 136,	// Upd Protocol
		/// </summary>
		Ethernet_UDPProtocol = 136,
		/// <summary>
		/// IODEVICETYPE_BC9191				= 137,	// BC9191 Etherent Slave
		/// </summary>
		BC9191 = 137,
		/// <summary>
		/// IODEVICETYPE_ENETPROTOCOL		= 138,	// Real-Time Ethernet Protocol (BK90xx, AX2000-B900)
		/// </summary>
		RTEThernet_BK90xx_AX2000B900 = 138
	}

	/// <summary>
	/// Disabled state type for the TreeItem
	/// </summary>
	/// <remarks>
	/// This is the CLS-compliant, corresponding type to TCatSysManagerLibs DISABLED_STATE
	/// </remarks>
	public enum DisabledState
	{
		/// <summary>
		/// The state is unknown/not initialized
		/// </summary>
		Unknown = -1,
		/// <summary>
		/// The item is not disabled
		/// </summary>
		NotDisabled = 0,
		/// <summary>
		/// The item is disabled itself
		/// </summary>
		Disabled = 1,
		/// <summary>
		/// A parent of the item is disabled (and implicitely the item itself)
		/// </summary>
		ParentDisabled = 2
	}

	/// <summary>
	/// 
	/// </summary>
	public enum IECLanguageType
	{
		/// <summary>
		/// 
		/// </summary>
		None  = TCatSysManagerLib.IECLANGUAGETYPES.IECLANGUAGE_NONE,

		/// <summary>
		/// 
		/// </summary>
		ST = TCatSysManagerLib.IECLANGUAGETYPES.IECLANGUAGE_ST,
		/// <summary>
		/// 
		/// </summary>
		IL = TCatSysManagerLib.IECLANGUAGETYPES.IECLANGUAGE_IL,
		/// <summary>
		/// 
		/// </summary>
		SFC = TCatSysManagerLib.IECLANGUAGETYPES.IECLANGUAGE_SFC,
		/// <summary>
		/// 
		/// </summary>
		FBD = TCatSysManagerLib.IECLANGUAGETYPES.IECLANGUAGE_FBD,
		/// <summary>
		/// 
		/// </summary>
		CFC = TCatSysManagerLib.IECLANGUAGETYPES.IECLANGUAGE_CFC,

		/// <summary>
		/// 
		/// </summary>
		LD = TCatSysManagerLib.IECLANGUAGETYPES.IECLANGUAGE_LD
	}

	/// <summary>
	/// Extension class for the <see cref="IECLanguageType"/>
	/// </summary>
	public static class IECLanguageTypeExtension
	{
		/// <summary>
		/// Converts the <see cref="IECLanguageType"/> to an Int32
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static int AsInt32(this IECLanguageType type)
		{
			return (int)type;
		}

		/// <summary>
		/// Converts the <see cref="IECLanguageType"/> to string identifyer
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static string AsString(this IECLanguageType type)
		{
			return type.ToString();
		}
	}

	/// <summary>
	/// Extension class for the <see cref="TreeItemType"/>.
	/// </summary>
	public static class TreeItemTypeExtension
	{
		/// <summary>
		/// Gets the description of the <see cref="TreeItemType"/>
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static string GetDescription(this TreeItemType type)
		{
			var field = type.GetType().GetField(type.ToString());
			var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attributes.Length > 0 ? attributes[0].Description : type.ToString();
		}

		/// <summary>
		/// Converts the <see cref="TreeItemType"/> to Int32.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static Int32 AsInt32(this TreeItemType type)
		{
			return (int)type;
		}
	}

	/// <summary>
	/// Extension class for the <see cref="TreeItemType"/>.
	/// </summary>
	public static class DeviceTypeExtension
	{
		/// <summary>
		/// Gets the description of the <see cref="DeviceType"/>
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static string GetDescription(this DeviceType type)
		{
			var field = type.GetType().GetField(type.ToString());
			var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attributes.Length > 0 ? attributes[0].Description : type.ToString();
		}

		/// <summary>
		/// Converts the <see cref="DeviceType"/> to Int32.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static Int32 AsInt32(this DeviceType type)
		{
			return (int)type;
		}
	}

	/// <summary>
	/// Extension class for <see cref="BoxType"/>
	/// </summary>
	public static class BoxTypeExtension
	{
		/// <summary>
		/// Gets the description of the <see cref="BoxType"/>.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static string GetDescription(this BoxType type)
		{
			var field = type.GetType().GetField(type.ToString());
			var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attributes.Length > 0 ? attributes[0].Description : type.ToString();
		}

		/// <summary>
		/// Gets the <see cref="BoxType"/> as Int32.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static Int32 AsInt32(this BoxType type)
		{
			return (int)type;
		}
	}

	/// <summary>
	/// Group type of variables
	/// </summary>
	public enum VarGroupType : int
	{
		/// <summary>
		/// Variable Group is unknown
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// Input variable group
		/// </summary>
		Input = 1,
		/// <summary>
		/// Output variable group
		/// </summary>
		Output = 2,
		/// <summary>
		/// Channel variable group
		/// </summary>
		Channel = 3,
	}
}

