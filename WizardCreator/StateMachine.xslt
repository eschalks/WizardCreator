<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="text" indent="no"/>

  <xsl:template match="statemachine" xml:space="default">
    <xsl:text>
using System;
using System.IO;
using WizardLibrary;

namespace NoNamespace
{
</xsl:text>

    <xsl:apply-templates select="state" />
    
    <xsl:text>
}
</xsl:text>
  </xsl:template>

  <xsl:template match="state">
    <xsl:if test="@initial">
      <xsl:text>   [InitialState]
</xsl:text>
    </xsl:if>
    <xsl:text>   public class </xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text>
   {</xsl:text>
    <xsl:apply-templates />
    <xsl:text>
   }
</xsl:text>
  </xsl:template>

  <xsl:template match="property">
    <xsl:if test="@visible">
      <xsl:text>
      [StateProperty(IsVisible = true)]</xsl:text>
    </xsl:if>
    <xsl:text>
      public </xsl:text>
    <xsl:value-of select="@type"/>
    <xsl:text> </xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text> {get; set;}</xsl:text>
  </xsl:template>

  <xsl:template match="entryAction">
    <xsl:text>
      public </xsl:text>
    <xsl:value-of select="../@name"/>
    <xsl:text>()
      {
         </xsl:text>
    <xsl:value-of select="current()"/>
    <xsl:text>    
      }
</xsl:text>
  </xsl:template>

  <xsl:template match="event">
    <xsl:text>
      [Event]
      public </xsl:text>
    <xsl:value-of select="@state"/>
    <xsl:text> </xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text>()
      {
</xsl:text>
    <xsl:apply-templates select="guard" />
    <xsl:value-of select="../exitAction"/>
    <xsl:text>        return new </xsl:text>
    <xsl:value-of select="@state"/>
    <xsl:text>();
      }</xsl:text>
  </xsl:template>

  <xsl:template match="guard">
    <xsl:text>
         if (!(</xsl:text>
    <xsl:value-of select="current()"/>
    <xsl:text>)) { throw new GuardException(&quot;</xsl:text>
    <xsl:value-of select="@error" />
    <xsl:text>&quot;); }
</xsl:text>
  </xsl:template>

  <xsl:template match="exitAction"></xsl:template>
</xsl:stylesheet>
