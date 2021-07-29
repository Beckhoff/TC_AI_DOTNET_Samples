using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.CSharp
{
    internal static class PouTemplates
    {
        public const string CFCPOU = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TcPlcObject Version=""1.0.0.0"">
  <POU Name=""CFCPou"" Id=""{80c1f0f1-bb28-4029-8cfe-5831316cfaab}"">
    <Declaration>
      <![CDATA[FUNCTION_BLOCK CFCPou
VAR_INPUT
        in1: INT;
        in3: INT;
END_VAR
VAR_OUTPUT
END_VAR
        out1: INT;
VAR
END_VAR
]]>
</Declaration>
    <Implementation>
      <CFC>
        <inVariable localId=""3"">
          <position x=""9"" y=""14"" />
          <connectionPointOut>
            <expression />
          </connectionPointOut>
          <expression>in1</expression>
        </inVariable>
        <connector localId=""2"" name="""">
          <position x=""0"" y=""0"" />
          <connectionPointIn>
            <connection refLocalId=""3"" formalParameter=""in1"" />
          </connectionPointIn>
        </connector>
        <inVariable localId=""5"">
          <position x=""9"" y=""18"" />
          <connectionPointOut>
            <expression />
          </connectionPointOut>
          <expression>in3</expression>
        </inVariable>
        <connector localId=""4"" name="""">
          <position x=""0"" y=""0"" />
          <connectionPointIn>
            <connection refLocalId=""5"" formalParameter=""in3"" />
          </connectionPointIn>
        </connector>
        <block localId=""1"" executionOrderId=""1"" typeName=""ADD"">
          <position x=""18"" y=""13"" />
          <inputVariables>
            <variable formalParameter=""In1"">
              <connectionPointIn>
                <relPosition x=""0"" y=""0"" />
                <connection refLocalId=""2"" />
              </connectionPointIn>
            </variable>
            <variable formalParameter=""In3"">
              <connectionPointIn>
                <relPosition x=""0"" y=""1"" />
                <connection refLocalId=""4"" />
              </connectionPointIn>
            </variable>
          </inputVariables>
          <inOutVariables />
          <outputVariables>
            <variable formalParameter=""Out1"">
              <connectionPointOut>
                <relPosition x=""0"" y=""0"" />
                <expression />
              </connectionPointOut>
            </variable>
          </outputVariables>
          <addData>
            <data name=""http://www.3s-software.com/plcopenxml/cfccalltype""
            handleUnknown=""implementation"">
              <CallType>operator</CallType>
            </data>
          </addData>
        </block>
        <connector localId=""0"" name="""">
          <position x=""0"" y=""0"" />
          <connectionPointIn>
            <connection refLocalId=""1"" formalParameter=""Out1"" />
          </connectionPointIn>
        </connector>
        <outVariable localId=""6"" executionOrderId=""2"">
          <position x=""33"" y=""14"" />
          <connectionPointIn>
            <relPosition x=""0"" y=""0"" />
            <connection refLocalId=""0"" />
          </connectionPointIn>
          <expression>out1</expression>
        </outVariable>
      </CFC>
    </Implementation>
    <Action Name=""Action1""
    Id=""{ab1a301d-e7a4-441e-9d6d-4373a287d6c1}"">
      <Implementation>
        <CFC />
      </Implementation>
    </Action>
    <Method Name=""Meth1""
    Id=""{9be20238-e41b-477d-bade-e7f7eaab2ce6}"">
      <Declaration>
        <![CDATA[METHOD Meth1 : BOOL
VAR_INPUT
END_VAR
]]>
</Declaration>
      <Implementation>
        <CFC />
      </Implementation>
    </Method>
    <Property Name=""Prop1""
    Id=""{26ff3948-3b1c-4ac3-ba7b-8859e59bbd17}"">
      <Declaration>
        <![CDATA[PROPERTY Prop1 : DWORD]]>
</Declaration>
      <Set Name=""Set"" Id=""{47d30dde-a0ca-442a-b3ab-cc503c3aa26f}"">
        <Declaration>
          <![CDATA[VAR
END_VAR
]]>
</Declaration>
        <Implementation>
          <ST>
            <![CDATA[]]>
</ST>
        </Implementation>
      </Set>
      <Get Name=""Get"" Id=""{4bb42f95-6540-4622-ac3b-dfab9cb6e377}"">
        <Declaration>
          <![CDATA[VAR
END_VAR
]]>
</Declaration>
        <Implementation>
          <ST>
            <![CDATA[]]>
</ST>
        </Implementation>
      </Get>
    </Property>
    <Transition Name=""TRANS1""
    Id=""{a1b9f763-d12d-419a-8758-203506e798f2}"">
      <Implementation>
        <ST>
          <![CDATA[]]>
</ST>
      </Implementation>
    </Transition>
  </POU>
</TcPlcObject>";

        public const string STPOU = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TcPlcObject Version=""1.0.0.0"">
  <POU Name=""STPOU"" Id=""{2a3c4600-99c5-441f-9ea3-7ab6e4a1549c}"">
    <Declaration>
      <![CDATA[FUNCTION_BLOCK STPOU
VAR_INPUT
        i1: INT;
        i2: INT;
END_VAR
VAR_OUTPUT
        out1: INT;
END_VAR
VAR
END_VAR
]]>
</Declaration>
    <Implementation>
      <ST>
        <![CDATA[out1 := i1 + i2;
]]>
</ST>
    </Implementation>
    <Action Name=""Action1""
    Id=""{617f7af8-e045-4962-bab8-5a88ae0b25ae}"">
      <Implementation>
        <ST>
          <![CDATA[]]>
</ST>
      </Implementation>
    </Action>
    <Method Name=""Method1""
    Id=""{30af4fae-d5c2-45a8-82c5-de33d464e37d}"">
      <Declaration>
        <![CDATA[METHOD PUBLIC Method1 : BOOL
VAR_INPUT
END_VAR
]]>
</Declaration>
      <Implementation>
        <CFC />
      </Implementation>
    </Method>
    <Property Name=""Prop1""
    Id=""{40bb37f6-b624-46c7-839e-811f21ca7b54}"">
      <Declaration>
        <![CDATA[PROPERTY PUBLIC Prop1 : DINT]]>
</Declaration>
      <Set Name=""Set"" Id=""{58e485c6-e6b5-440a-a26b-49769fa39629}"">
        <Declaration>
          <![CDATA[VAR
END_VAR
]]>
</Declaration>
        <Implementation>
          <ST>
            <![CDATA[]]>
</ST>
        </Implementation>
      </Set>
      <Get Name=""Get"" Id=""{d88efab0-f215-4d60-ba43-ea96029d1ca9}"">
        <Declaration>
          <![CDATA[VAR
END_VAR
]]>
</Declaration>
        <Implementation>
          <ST>
            <![CDATA[]]>
</ST>
        </Implementation>
      </Get>
    </Property>
    <Transition Name=""Transition1""
    Id=""{ef652342-5185-486f-8925-629e0e5583dd}"">
      <Implementation>
        <ST>
          <![CDATA[]]>
</ST>
      </Implementation>
    </Transition>
  </POU>
</TcPlcObject>
";
    }
}
