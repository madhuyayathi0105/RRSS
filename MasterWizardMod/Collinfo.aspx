<%@ Page Title="" Language="C#" MasterPageFile="~/MasterWizardMod/MasterWizard.master"
    AutoEventWireup="true" CodeFile="Collinfo.aspx.cs" Inherits="Collinfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .maindivstylesize
        {
            height: 3000px;
            width: 1200px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <center>
        <span class="fontstyleheader" style="color: Green;">College Information</span>
    </center>
    <br>
    <center>
        <asp:UpdatePanel ID='UpdGridStudent' runat="server">
            <ContentTemplate>
                <div class="maindivstylesize">
                    <table style="top: 130px; margin-left: -250px; width: 850px;">
                        <%-- <table>--%>
                        <tr>
                            <td>
                                <asp:Label ID="lblcollege1" runat="server" Text="Select College" Font-Names="Book Antiqua"
                                    Font-Size="Medium" Font-Bold="true"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlcollege1" runat="server" Width="520px" Font-Bold="true"
                                    Font-Names="Book Antiqua" Font-Size="Medium" CssClass="textbox ddlstyle ddlheight3"
                                    OnSelectedIndexChanged="ddlcollege1_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblCollName" runat="server" Text="College Name" Font-Names="Book Antiqua"
                                    Font-Size="Medium" Font-Bold="true"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCollName" runat="server" Width="520px" CssClass="textbox txtheight1"
                                    Style="font-weight: bold; font-family: book antiqua; font-size: medium; margin-left: -2px;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblUniver" runat="server" Text="University" Font-Names="Book Antiqua"
                                    Font-Size="Medium" Font-Bold="true"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtUniver" runat="server" Width="520px" CssClass="textbox txtheight1"
                                    Style="font-weight: bold; font-family: book antiqua; font-size: medium; margin-left: -2px;"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <div>
                        <asp:Panel ID="panel2" runat="server" CssClass="cpHeader" BackColor="#719DDB" Width="1100px">
                            <asp:Label ID="Label28" Text="Affiliation Details" runat="server" Font-Size="Large"
                                Font-Bold="true" Font-Names="Book Antiqua" />
                            <asp:Image ID="image1" runat="server" CssClass="cpimage" ImageUrl="../images/right.jpeg"
                                ImageAlign="Right" />
                        </asp:Panel>
                    </div>
                    <asp:Panel ID="Panel3" runat="server" Height="400px">
                        <fieldset style="top: 16px; height: auto; width: 1070px; border-color: Olive; background-color: #99ffcc;
                            border-bottom-width: 2px;">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="Total No.of.Affiliation" Font-Names="Book Antiqua"
                                            Font-Size="Medium" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="TextBox1" runat="server" Width="90px" CssClass="textbox txtheight1"
                                            Style="font-weight: bold; font-family: book antiqua; font-size: medium; margin-left: -2px;"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblColStYr" runat="server" Text="College Started Year" Font-Names="Book Antiqua"
                                            Font-Size="Medium" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtColStYr" runat="server" Width="90px" CssClass="textbox txtheight1"
                                            Style="font-weight: bold; font-family: book antiqua; font-size: medium; margin-left: -2px;"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:GridView ID="gridAffiliation" runat="server" Style="margin-bottom: 15px; margin-top: 15px;
                                            width: 950px;" Font-Names="Times New Roman" AutoGenerateColumns="false" BackColor="AliceBlue">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lnkSno" runat="server" Text="<%# Container.DisplayIndex+1 %>" OnClick="lnkAttMark11"
                                                            ForeColor="Black" Font-Underline="false"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="30px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Affiliated By">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtAffiliation" runat="server" Text='<%# Eval("Affiliatedby") %>'
                                                            Style="text-align: left" Width="600px"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Affiliated Year">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtaffYr" runat="server" MaxLength="4" Text='<%# Eval("AffiliatedYR") %>'
                                                            Style="text-align: center" Width="80px"></asp:TextBox>
                                                        <asp:FilteredTextBoxExtender ID="FilteredTextBoxExtender56" runat="server" TargetControlID="txtaffYr"
                                                            FilterType="Numbers,Custom">
                                                        </asp:FilteredTextBoxExtender>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <HeaderStyle BackColor="#0CA6CA" ForeColor="Black" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </asp:Panel>
                    <asp:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server" TargetControlID="panel3"
                        CollapseControlID="panel2" ExpandControlID="panel2" Collapsed="true" TextLabelID="Label28"
                        CollapsedSize="0" ImageControlID="imagecontactcollaps" CollapsedImage="../images/right.jpeg"
                        ExpandedImage="../images/down.jpeg">
                    </asp:CollapsiblePanelExtender>
                    <br />
                    <div>
                        <asp:Panel ID="panelcontactcollaps" runat="server" CssClass="cpHeader" BackColor="#719DDB"
                            Width="1100px">
                            <asp:Label ID="lblcontactcollaps" Text="Contact" runat="server" Font-Size="Large"
                                Font-Bold="true" Font-Names="Book Antiqua" />
                            <asp:Image ID="imagecontactcollaps" runat="server" CssClass="cpimage" ImageUrl="../images/right.jpeg"
                                ImageAlign="Right" />
                        </asp:Panel>
                    </div>
                    <asp:Panel ID="Panel5" runat="server" Height="400px">
                        <table>
                            <tr>
                                <td>
                                    <asp:Panel ID="permantpanel" runat="server" Style="border-color: Gray; border-width: thin;">
                                        <table class="tabl" style="width: 450px; height: 300px; background-color: #ffccff;">
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblCategory" runat="server" Text="Category" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td class="style25">
                                                                <asp:RadioButton ID="rbrmale" Text="Autonomous" runat="server" GroupName="Category"
                                                                    Font-Names="Book Antiqua" Font-Size="Medium" Checked="true" />
                                                                <asp:RadioButton ID="rbrfemale" Text="Affilated" runat="server" GroupName="Category"
                                                                    Font-Names="Book Antiqua" Font-Size="Medium" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblpaddress" runat="server" Text="Address" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="txt_paddress" runat="server" Width="250px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                    <asp:FilteredTextBoxExtender ID="fte1" runat="server" TargetControlID="txt_paddress"
                                                        FilterType="Custom,Lowercaseletters,Uppercaseletters,Numbers" ValidChars="/,.() ">
                                                    </asp:FilteredTextBoxExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblStreet" runat="server" Text="Street" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox3" runat="server" Width="250px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblCity" runat="server" Text="City" Font-Names="Book Antiqua" Style="margin-left: 20px;"
                                                        Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox4" runat="server" Width="250px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblDistrict" runat="server" Text="District" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox5" runat="server" Width="250px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblState" runat="server" Text="State" Font-Names="Book Antiqua" Style="margin-left: 20px;"
                                                        Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox6" runat="server" Width="250px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblPinCode" runat="server" Text="Pincode" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox7" runat="server" Width="250px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblPhone" runat="server" Text="Phone No" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox8" runat="server" Width="250px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblFax" runat="server" Text="Fax" Font-Names="Book Antiqua" Style="margin-left: 20px;"
                                                        Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox15" runat="server" Width="250px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblEmail" runat="server" Text="E-Mail" Font-Names="Book Antiqua" Style="margin-left: 20px;"
                                                        Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox16" runat="server" Width="250px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblWebSite" runat="server" Text="Web Site" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox17" runat="server" Width="250px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td>
                                    <asp:Panel ID="Panel1" runat="server" Style="border-color: Gray; border-width: thin;">
                                        <table class="tabl" style="width: 640px; height: 400px; background-color: #ffccff;">
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblAcr" runat="server" Text="Acronym" Font-Names="Book Antiqua" Style="margin-left: 20px;"
                                                        Font-Size="Medium"></asp:Label>
                                                    <asp:TextBox ID="TextBox19" runat="server" Width="70px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;"></asp:TextBox>
                                                    <asp:Label ID="lblColCode" runat="server" Text="College Code" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                    <asp:TextBox ID="TextBox20" runat="server" Width="70px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblChancellor" runat="server" Text="Chancellor" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox2" runat="server" Width="200px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="btnAdd" runat="server"  Height="30px" ImageUrl="~/college/Ques Icon.png" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblViceChancellor" runat="server" Text="Vice-Chancellor" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox9" runat="server" Width="200px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                                <td>
                                                     <asp:ImageButton ID="ImageButton1" runat="server"  Height="30px" ImageUrl="~/college/Ques Icon.png" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblChairman" runat="server" Text="Correspondent/Chairman" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox10" runat="server" Width="200px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton2" runat="server"  Height="30px" ImageUrl="~/college/Ques Icon.png" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblSecretary" runat="server" Text="Secretary" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox11" runat="server" Width="200px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                                <td>
                                                  <asp:ImageButton ID="ImageButton3" runat="server"  Height="30px" ImageUrl="~/college/Ques Icon.png" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblPrincipal" runat="server" Text="Principal/Registrar" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox12" runat="server" Width="200px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton4" runat="server"  Height="30px" ImageUrl="~/college/Ques Icon.png" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lbladmin" runat="server" Text="Vice-principal/Cheif     Administrator Officeer"
                                                        Font-Names="Book Antiqua" Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox13" runat="server" Width="200px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton5" runat="server"  Height="30px" ImageUrl="~/college/Ques Icon.png" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style5">
                                                    <asp:Label ID="lblEstNo" runat="server" Text="Establishment No" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox14" runat="server" Width="200px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblCOE" runat="server" Text="Name of Controller of Examimation" Font-Names="Book Antiqua"
                                                        Style="margin-left: 20px;" Font-Size="Medium"></asp:Label>
                                                </td>
                                                <td class="style25">
                                                    <asp:TextBox ID="TextBox18" runat="server" Width="200px" MaxLength="100" Font-Names="Book Antiqua"
                                                        CssClass="textbox txtheight1" Style="font-weight: bold; font-family: book antiqua;
                                                        font-size: medium; margin-left: 20px;" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:CollapsiblePanelExtender ID="CollapsiblePanelExtender14" runat="server" TargetControlID="Panel5"
                        CollapseControlID="panelcontactcollaps" ExpandControlID="panelcontactcollaps"
                        Collapsed="true" TextLabelID="lblcontactcollaps" CollapsedSize="0" ImageControlID="imagecontactcollaps"
                        CollapsedImage="../images/right.jpeg" ExpandedImage="../images/down.jpeg">
                    </asp:CollapsiblePanelExtender>
                    <br />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </center>
</asp:Content>
