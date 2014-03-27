
using System;
using System.IO;

namespace NoNamespace
{

}
e="Configuration">
    <event name="Cancel" state="Cancelled" />
  </state>
  <state name="Cancelled" />
  <state name="Finished" />
  <state name="Introduction">
    <event name="Next" state="Components" />
    <event name="Cancel" state="Cancelled" />
  </state>
  <state name="Components">
    <property type="System.Boolean" name="Binary Files" />
    <property type="System.Boolean" name="Source Code" />
    <property type="System.Boolean" name="HAL 2012" />
    <property type="System.Boolean" name="Documentation" />
    <event name="Back" state="Introduction" />
    <event name="Next" state="Finished" />
    <event name="Cancel" state="Cancelled" />
  </state>
</statemachine>