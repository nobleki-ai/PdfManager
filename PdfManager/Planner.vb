Imports System.IO

Public Enum PdfOperation
    Combine
    MergePages
    ExtractPages
    Encrypt
    Rotate
    Stamp
    Compress
    ''' <summary>Pages put in a new order, or left out, in the page thumbnail view.</summary>
    ArrangePages
    ''' <summary>Removes the password and any limits on printing, copying or editing (on the Encrypt page).</summary>
    Decrypt
End Enum

''' <summary>One page of one PDF in the list, as shown in the page thumbnail view.</summary>
Public NotInheritable Class PageRef
    Public Sub New(item As PdfItem, pageNumber As Integer)
        Me.Item = item
        Me.PageNumber = pageNumber
    End Sub
    Public ReadOnly Property Item As PdfItem
    ''' <summary>Counting from 1.</summary>
    Public ReadOnly Property PageNumber As Integer

    Public Overrides Function ToString() As String
        Return $"{Item.FileName} p{PageNumber}"
    End Function
End Class

Public Enum StampMode
    ''' <summary>A page of another PDF on top of each page.</summary>
    StampOnly
    ''' <summary>A page of another PDF behind each page.</summary>
    BackgroundOnly
    ''' <summary>A stamp on top and a background behind, in one go.</summary>
    Both
End Enum

''' <summary>How hard Reduce Size tries. Normal is lossless; the others re-save photos as JPEG at lower quality.</summary>
Public Enum CompressLevel
    Normal
    Medium
    High
    Extreme
End Enum

Public Enum ExtractOutput
    OnePdf
    EachPage
    EveryNPages
End Enum

''' <summary>The part of the window a problem belongs to, so it can be pointed out.</summary>
Public Enum RequestField
    FileList
    OutputFolder
    OutputName
    ExtractPages
    OpenPassword
    ConfirmPassword
    PermissionsPassword
    RotatePages
    StampFile
    BackgroundFile
    StampPages
    ArrangePages
    ''' <summary>Step 3's "Protect with a password" settings.</summary>
    ExtraPassword
    ''' <summary>Step 3's "Add a stamp / background" settings.</summary>
    ExtraStamp
End Enum

''' <summary>A password, and optional limits, for new PDFs.</summary>
Public Class ProtectionSettings
    Public Property OpenPassword As String = ""
    Public Property ConfirmPassword As String = ""
    Public Property PermissionsPassword As String = ""
    Public Property AllowPrinting As Boolean = True
    Public Property AllowCopying As Boolean = True
    Public Property AllowEditing As Boolean = True

    ''' <summary>The password that opens the result: the one to open it, else the permissions password.</summary>
    Public ReadOnly Property ReadPassword As String
        Get
            Return If(OpenPassword.Length > 0, OpenPassword, PermissionsPassword)
        End Get
    End Property

    Public Function Copy() As ProtectionSettings
        Return DirectCast(MemberwiseClone(), ProtectionSettings)
    End Function
End Class

''' <summary>A stamp on top and/or a background behind the pages of new PDFs.</summary>
Public Class LayerSettings
    Public Property Mode As StampMode = StampMode.StampOnly
    Public Property StampItem As PdfItem
    Public Property StampPageNumber As Integer = 1
    Public Property BackgroundItem As PdfItem
    Public Property BackgroundPageNumber As Integer = 1
    ''' <summary>The pages of the new PDF that get them; blank or All for every page.</summary>
    Public Property Pages As String = ""

    Public ReadOnly Property UsesStamp As Boolean
        Get
            Return Mode <> StampMode.BackgroundOnly
        End Get
    End Property

    Public ReadOnly Property UsesBackground As Boolean
        Get
            Return Mode <> StampMode.StampOnly
        End Get
    End Property

    Public Function Copy() As LayerSettings
        Return DirectCast(MemberwiseClone(), LayerSettings)
    End Function
End Class

''' <summary>Everything the person chose in the window.</summary>
Public Class OperationRequest
    Public Property Operation As PdfOperation
    Public Property Items As IList(Of PdfItem) = New List(Of PdfItem)
    Public Property OutputFolder As String = ""
    Public Property OutputName As String = ""

    Public Property KeepFirstFileDetails As Boolean

    Public Property ExtractPages As String = ""
    Public Property ExtractOutput As ExtractOutput = ExtractOutput.OnePdf
    Public Property SplitEvery As Integer = 2

    Public Property OpenPassword As String = ""
    Public Property ConfirmPassword As String = ""
    Public Property PermissionsPassword As String = ""
    Public Property AllowPrinting As Boolean = True
    Public Property AllowCopying As Boolean = True
    Public Property AllowEditing As Boolean = True

    ''' <summary>90 turns clockwise, -90 anticlockwise, 180 upside down.</summary>
    Public Property RotateAngle As Integer = 90
    Public Property RotatePages As String = ""

    ''' <summary>Stamp only, background only, or both at once.</summary>
    Public Property StampMode As StampMode = StampMode.StampOnly
    ''' <summary>The PDF whose page is laid on top of each page.</summary>
    Public Property StampItem As PdfItem
    ''' <summary>Which page of the stamp PDF to use, counting from 1.</summary>
    Public Property StampPageNumber As Integer = 1
    ''' <summary>The PDF whose page is laid behind each page.</summary>
    Public Property BackgroundItem As PdfItem
    ''' <summary>Which page of the background PDF to use, counting from 1.</summary>
    Public Property BackgroundPageNumber As Integer = 1
    ''' <summary>The pages that get the stamp and/or background.</summary>
    Public Property StampPages As String = ""

    Public ReadOnly Property UsesStamp As Boolean
        Get
            Return StampMode <> StampMode.BackgroundOnly
        End Get
    End Property

    Public ReadOnly Property UsesBackground As Boolean
        Get
            Return StampMode <> StampMode.StampOnly
        End Get
    End Property

    Public Property CompressLevel As CompressLevel = CompressLevel.Medium

    ''' <summary>Step 3: also protect every new PDF with a password (Nothing for no). Not used by Encrypt / Decrypt.</summary>
    Public Property ExtraProtection As ProtectionSettings
    ''' <summary>Step 3: also add a stamp and/or background to every new PDF (Nothing for no). Not used by Stamp &amp; Background.</summary>
    Public Property ExtraLayers As LayerSettings

    ''' <summary>Whether the password from step 3 applies: Encrypt / Decrypt has its own password settings.</summary>
    Public ReadOnly Property UsesExtraProtection As Boolean
        Get
            Return ExtraProtection IsNot Nothing AndAlso Operation <> PdfOperation.Encrypt AndAlso Operation <> PdfOperation.Decrypt
        End Get
    End Property

    ''' <summary>Whether the stamp / background from step 3 applies: Stamp &amp; Background has its own.</summary>
    Public ReadOnly Property UsesExtraLayers As Boolean
        Get
            Return ExtraLayers IsNot Nothing AndAlso Operation <> PdfOperation.Stamp
        End Get
    End Property

    ''' <summary>The Encrypt page's password settings.</summary>
    Public Function Protection() As ProtectionSettings
        Return New ProtectionSettings With {
            .OpenPassword = OpenPassword, .ConfirmPassword = ConfirmPassword, .PermissionsPassword = PermissionsPassword,
            .AllowPrinting = AllowPrinting, .AllowCopying = AllowCopying, .AllowEditing = AllowEditing}
    End Function

    ''' <summary>
    ''' Arrange Pages: the pages to keep, in their new order. Nothing means every page of every
    ''' PDF in the list, in list order (the page view hasn't been changed).
    ''' </summary>
    Public Property ArrangedPages As IList(Of PageRef)

    ''' <summary>The pages Arrange Pages will save: the arranged ones, or every page in list order.</summary>
    Public Function PagesToArrange() As List(Of PageRef)
        If ArrangedPages IsNot Nothing Then Return ArrangedPages.ToList()
        Return AllPages(Items)
    End Function

    ''' <summary>Every page of the given PDFs, in order.</summary>
    Public Shared Function AllPages(items As IEnumerable(Of PdfItem)) As List(Of PageRef)
        Dim pages As New List(Of PageRef)
        For Each item In items
            For page = 1 To item.PageCount.GetValueOrDefault(0)
                pages.Add(New PageRef(item, page))
            Next
        Next
        Return pages
    End Function
End Class

Public Class ValidationIssue
    Public Sub New(message As String, field As RequestField)
        Me.Message = message
        Me.Field = field
    End Sub
    Public ReadOnly Property Message As String
    Public ReadOnly Property Field As RequestField
End Class

''' <summary>A result file. A split result becomes several files that all start with this name.</summary>
Public Class OutputTarget
    Public Sub New(baseName As String, isSplit As Boolean)
        Me.BaseName = baseName
        Me.IsSplit = isSplit
    End Sub
    Public ReadOnly Property BaseName As String
    Public ReadOnly Property IsSplit As Boolean
End Class

''' <summary>One run of QPDF.</summary>
Public Class QpdfJob
    Public Property Description As String
    Public Property Target As OutputTarget
    Public Property Arguments As New List(Of String)
    ''' <summary>The password that opens this job's result, when it differs from the plan's.</summary>
    Public Property ReadPassword As String
End Class

Public Class OperationPlan
    Public Property OutputFolder As String
    Public Property Jobs As New List(Of QpdfJob)
    ''' <summary>The password that opens the results, used to confirm that they read back correctly.</summary>
    Public Property ResultPassword As String
    ''' <summary>For Reduce Size: the total size of the originals, to report how much was saved.</summary>
    Public Property InputBytes As Long?
End Class

''' <summary>Decides what QPDF must be told. Nothing here touches a file except to look for clashes.</summary>
Public Module Planner

    ''' <summary>Stands in for the output file until the runner knows its working folder.</summary>
    Public Const OutputToken As String = "<<output>>"

    Private ReadOnly ReservedNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    }

    ''' <summary>The name typed, without any .pdf on the end.</summary>
    Public Function CleanName(name As String) As String
        Dim result = If(name, "").Trim()
        If result.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) Then
            result = result.Substring(0, result.Length - 4).TrimEnd()
        End If
        Return result
    End Function

    ''' <summary>Returns the first problem with the request, or Nothing when it can run.</summary>
    Public Function Validate(r As OperationRequest) As ValidationIssue
        Dim issue = ValidateFiles(r)
        If issue Is Nothing Then issue = ValidateOptions(r)
        If issue Is Nothing Then issue = ValidateExtras(r)
        If issue Is Nothing Then issue = ValidateOutput(r)
        Return issue
    End Function

    ''' <summary>Step 3's extra password and stamp / background, when ticked.</summary>
    Public Function ValidateExtras(r As OperationRequest) As ValidationIssue
        If r.UsesExtraProtection Then
            Dim issue = ValidateProtection(r.ExtraProtection, forExtras:=True)
            If issue IsNot Nothing Then Return New ValidationIssue("Protect with a password: " & issue.Message, issue.Field)
        End If
        If r.UsesExtraLayers Then
            Dim issue = ValidateLayers(r.ExtraLayers)
            If issue IsNot Nothing Then Return issue
        End If
        Return Nothing
    End Function

    ''' <summary>The stamp / background settings from step 3. The new PDF's page count isn't known yet, so pages are only checked for mistakes.</summary>
    Public Function ValidateLayers(layers As LayerSettings) As ValidationIssue
        Dim issue As ValidationIssue = Nothing
        If layers.UsesStamp Then issue = ValidateLayer(layers.StampItem, layers.StampPageNumber, "stamp", RequestField.ExtraStamp)
        If issue Is Nothing AndAlso layers.UsesBackground Then
            issue = ValidateLayer(layers.BackgroundItem, layers.BackgroundPageNumber, "background", RequestField.ExtraStamp)
        End If
        If issue IsNot Nothing Then Return New ValidationIssue("Add a stamp / background: " & issue.Message, RequestField.ExtraStamp)
        Dim range = PageRange.Parse(layers.Pages)
        If Not range.IsValid Then Return New ValidationIssue("Add a stamp / background: " & range.ErrorMessage, RequestField.ExtraStamp)
        Return Nothing
    End Function

    Private Function ValidateFiles(r As OperationRequest) As ValidationIssue
        If r.Items.Count = 0 Then
            If r.Operation = PdfOperation.ExtractPages Then
                Return New ValidationIssue("Select the file to take pages from, or choose ""Every file in the list"".", RequestField.FileList)
            End If
            If r.Operation = PdfOperation.MergePages Then
                Return New ValidationIssue("Tick at least one PDF in the Use column.", RequestField.FileList)
            End If
            Return New ValidationIssue("Add at least one PDF first.", RequestField.FileList)
        End If

        If r.Operation = PdfOperation.Combine AndAlso r.Items.Count < 2 Then
            Return New ValidationIssue("Combining needs at least two PDFs in the list.", RequestField.FileList)
        End If

        For Each item In r.Items
            Select Case item.State
                Case PdfState.Checking
                    Return New ValidationIssue($"Still reading {item.FileName}. Try again in a moment.", RequestField.FileList)
                Case PdfState.PasswordNeeded
                    Return New ValidationIssue($"{item.FileName} needs its password. Double-click it in the list.", RequestField.FileList)
                Case PdfState.Unreadable
                    Return New ValidationIssue($"{item.FileName} can't be read. Remove it from the list to continue.", RequestField.FileList)
            End Select
        Next
        Return Nothing
    End Function

    Private Function ValidateOptions(r As OperationRequest) As ValidationIssue
        Select Case r.Operation
            Case PdfOperation.MergePages
                For Each item In r.Items
                    Dim range = PageRange.Parse(item.PagesText, item.PageCount)
                    If Not range.IsValid Then
                        Return New ValidationIssue($"{item.FileName}: {range.ErrorMessage}", RequestField.FileList)
                    End If
                Next

            Case PdfOperation.ExtractPages
                If String.IsNullOrWhiteSpace(r.ExtractPages) Then
                    Return New ValidationIssue("Type the pages to extract, for example 1-3, 5, 8-end.", RequestField.ExtractPages)
                End If
                For Each item In r.Items
                    Dim range = PageRange.Parse(r.ExtractPages, item.PageCount)
                    If Not range.IsValid Then
                        Return New ValidationIssue($"{item.FileName}: {range.ErrorMessage}", RequestField.ExtractPages)
                    End If
                Next

            Case PdfOperation.Encrypt
                Return ValidatePasswords(r)

            Case PdfOperation.Rotate
                For Each item In r.Items
                    Dim range = PageRange.Parse(r.RotatePages, item.PageCount)
                    If Not range.IsValid Then
                        Return New ValidationIssue($"{item.FileName}: {range.ErrorMessage}", RequestField.RotatePages)
                    End If
                Next

            Case PdfOperation.Stamp
                Return ValidateStamp(r)

            Case PdfOperation.ArrangePages
                Dim pages = r.PagesToArrange()
                If pages.Count = 0 Then
                    Return New ValidationIssue("Every page was removed. Open the page view and use Start Over to bring them back.", RequestField.ArrangePages)
                End If
                Dim inList As New HashSet(Of PdfItem)(r.Items)
                For Each page In pages
                    If Not inList.Contains(page.Item) Then
                        Return New ValidationIssue($"{page.Item.FileName} is no longer in the list. Open the page view to check the pages.", RequestField.ArrangePages)
                    End If
                    If page.PageNumber < 1 OrElse page.PageNumber > page.Item.PageCount.GetValueOrDefault(0) Then
                        Return New ValidationIssue($"{page.Item.FileName} has no page {page.PageNumber}. Open the page view to check the pages.", RequestField.ArrangePages)
                    End If
                Next
        End Select
        Return Nothing
    End Function

    Private Function ValidateStamp(r As OperationRequest) As ValidationIssue
        Dim issue As ValidationIssue = Nothing
        If r.UsesStamp Then issue = ValidateLayer(r.StampItem, r.StampPageNumber, "stamp", RequestField.StampFile)
        If issue Is Nothing AndAlso r.UsesBackground Then
            issue = ValidateLayer(r.BackgroundItem, r.BackgroundPageNumber, "background", RequestField.BackgroundFile)
        End If
        If issue IsNot Nothing Then Return issue

        For Each item In r.Items
            Dim range = PageRange.Parse(r.StampPages, item.PageCount)
            If Not range.IsValid Then
                Return New ValidationIssue($"{item.FileName}: {range.ErrorMessage}", RequestField.StampPages)
            End If
        Next
        Return Nothing
    End Function

    ''' <summary>Checks the stamp or the background PDF and the page chosen from it.</summary>
    Private Function ValidateLayer(layer As PdfItem, pageNumber As Integer, what As String, field As RequestField) As ValidationIssue
        If layer Is Nothing Then
            Return New ValidationIssue($"Choose the PDF to use as the {what}.", field)
        End If
        Select Case layer.State
            Case PdfState.Checking
                Return New ValidationIssue($"Still reading {layer.FileName}. Try again in a moment.", field)
            Case PdfState.PasswordNeeded
                Return New ValidationIssue($"The {what} PDF needs its password. Choose it again to type the password.", field)
            Case PdfState.Unreadable
                Return New ValidationIssue($"The {what} PDF can't be read. Choose another one.", field)
        End Select
        If layer.PageCount.HasValue AndAlso pageNumber > layer.PageCount.Value Then
            Dim pages = If(layer.PageCount.Value = 1, "1 page", $"{layer.PageCount.Value} pages")
            Return New ValidationIssue($"The {what} PDF has only {pages}, so it has no page {pageNumber}.", field)
        End If
        Return Nothing
    End Function

    Private Function ValidatePasswords(r As OperationRequest) As ValidationIssue
        Return ValidateProtection(r.Protection(), forExtras:=False)
    End Function

    ''' <summary>Checks a password and its limits, for the Encrypt page or for step 3's extra password.</summary>
    Public Function ValidateProtection(p As ProtectionSettings, Optional forExtras As Boolean = False) As ValidationIssue
        Dim at = Function(field As RequestField) If(forExtras, RequestField.ExtraPassword, field)
        If p.OpenPassword <> p.ConfirmPassword Then
            Return New ValidationIssue("The two passwords don't match.", at(RequestField.ConfirmPassword))
        End If
        If p.OpenPassword.Length = 0 AndAlso p.PermissionsPassword.Length = 0 Then
            Return New ValidationIssue("Type a password.", at(RequestField.OpenPassword))
        End If
        If p.OpenPassword.Any(AddressOf Char.IsControl) Then
            Return New ValidationIssue("That password contains a character that can't be used.", at(RequestField.OpenPassword))
        End If
        If p.PermissionsPassword.Any(AddressOf Char.IsControl) Then
            Return New ValidationIssue("That password contains a character that can't be used.", at(RequestField.PermissionsPassword))
        End If

        Dim restricted = Not (p.AllowPrinting AndAlso p.AllowCopying AndAlso p.AllowEditing)
        If restricted AndAlso p.PermissionsPassword.Length = 0 Then
            Return New ValidationIssue("Limits on printing, copying or editing need a permissions password.", at(RequestField.PermissionsPassword))
        End If
        If restricted AndAlso p.PermissionsPassword = p.OpenPassword Then
            Return New ValidationIssue("The permissions password must differ from the password to open, or the limits won't apply.", at(RequestField.PermissionsPassword))
        End If
        Return Nothing
    End Function

    Private Function ValidateOutput(r As OperationRequest) As ValidationIssue
        Dim folder = If(r.OutputFolder, "").Trim()
        If folder.Length = 0 Then
            Return New ValidationIssue("Choose the folder to save in.", RequestField.OutputFolder)
        End If
        If folder.IndexOfAny(Path.GetInvalidPathChars()) >= 0 OrElse Not Path.IsPathFullyQualified(folder) Then
            Return New ValidationIssue("That folder isn't valid. Use Browse to choose one.", RequestField.OutputFolder)
        End If

        Dim name = CleanName(r.OutputName)
        If name.Length = 0 Then
            Return New ValidationIssue("Type a name for the new PDF.", RequestField.OutputName)
        End If
        If name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 OrElse name.EndsWith("."c) Then
            Return New ValidationIssue("A file name can't contain any of these: \ / : * ? "" < > | and can't end with a dot.", RequestField.OutputName)
        End If
        If ReservedNames.Contains(name) Then
            Return New ValidationIssue($"Windows doesn't allow a file named {name}.", RequestField.OutputName)
        End If

        ' Never let a result replace one of the files it was made from.
        Dim inputs As New HashSet(Of String)(r.Items.Select(Function(i) i.Path), StringComparer.OrdinalIgnoreCase)
        For Each target In OutputTargets(r)
            If target.IsSplit Then
                Dim prefix = target.BaseName & "-"
                For Each inputPath In inputs
                    If String.Equals(Path.GetDirectoryName(inputPath), FullFolder(folder), StringComparison.OrdinalIgnoreCase) AndAlso
                       Path.GetFileName(inputPath).StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                        Return New ValidationIssue($"The new files could replace {Path.GetFileName(inputPath)}. Choose another name or folder.", RequestField.OutputName)
                    End If
                Next
            Else
                Dim fullPath = Path.Combine(FullFolder(folder), target.BaseName & ".pdf")
                If inputs.Contains(fullPath) Then
                    Return New ValidationIssue($"That would replace {target.BaseName}.pdf, which is one of the files you added. Choose another name or folder.", RequestField.OutputName)
                End If
            End If
        Next
        Return Nothing
    End Function

    Private Function FullFolder(folder As String) As String
        Return Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder.Trim()))
    End Function

    ''' <summary>The results the request will produce, named as they will appear in the output folder.</summary>
    Public Function OutputTargets(r As OperationRequest) As List(Of OutputTarget)
        Dim name = CleanName(r.OutputName)
        Dim results As New List(Of OutputTarget)
        Dim used As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Dim addUnique =
            Sub(baseName As String, isSplit As Boolean)
                Dim candidate = baseName
                Dim counter = 2
                While Not used.Add(candidate)
                    candidate = $"{baseName} ({counter})"
                    counter += 1
                End While
                results.Add(New OutputTarget(candidate, isSplit))
            End Sub

        Select Case r.Operation
            Case PdfOperation.ExtractPages
                If r.ExtractOutput = ExtractOutput.OnePdf Then
                    addUnique(name, False)
                ElseIf r.Items.Count <= 1 Then
                    addUnique(name, True)
                Else
                    For Each item In r.Items
                        addUnique($"{name}-{item.BaseName}", True)
                    Next
                End If

            Case PdfOperation.Encrypt, PdfOperation.Decrypt, PdfOperation.Rotate, PdfOperation.Stamp, PdfOperation.Compress
                ' Each PDF in the list gets its own result.
                If r.Items.Count <= 1 Then
                    addUnique(name, False)
                Else
                    For Each item In r.Items
                        addUnique($"{item.BaseName}-{name}", False)
                    Next
                End If

            Case Else
                addUnique(name, False)
        End Select
        Return results
    End Function

    ''' <summary>A short description of what will be created, shown under the output boxes.</summary>
    Public Function DescribeOutputs(r As OperationRequest) As String
        If String.IsNullOrWhiteSpace(CleanName(r.OutputName)) Then Return ""
        Dim targets = OutputTargets(r)
        If targets.Count = 0 Then Return ""

        Dim pattern =
            Function(t As OutputTarget) If(t.IsSplit, $"{t.BaseName}-01.pdf, {t.BaseName}-02.pdf …", $"{t.BaseName}.pdf")

        If targets.Count = 1 Then Return $"Will create: {pattern(targets(0))}"
        Dim preview = String.Join(", ", targets.Take(2).Select(pattern))
        Dim more = If(targets.Count > 2, $" and {targets.Count - 2} more", "")
        Return $"Will create a result for each file: {preview}{more}"
    End Function

    ''' <summary>Existing files the results would replace.</summary>
    Public Function FindConflicts(r As OperationRequest) As List(Of String)
        Dim conflicts As New List(Of String)
        Dim folder = FullFolder(r.OutputFolder)
        If Not Directory.Exists(folder) Then Return conflicts

        For Each target In OutputTargets(r)
            If target.IsSplit Then
                conflicts.AddRange(Directory.GetFiles(folder, target.BaseName & "-*.pdf").Select(AddressOf Path.GetFileName))
            ElseIf File.Exists(Path.Combine(folder, target.BaseName & ".pdf")) Then
                conflicts.Add(target.BaseName & ".pdf")
            End If
        Next
        Return conflicts
    End Function

    ''' <summary>Turns a valid request into the QPDF runs that carry it out.</summary>
    Public Function Plan(r As OperationRequest) As OperationPlan
        Dim result As New OperationPlan With {.OutputFolder = FullFolder(r.OutputFolder)}
        Dim targets = OutputTargets(r)

        Select Case r.Operation
            Case PdfOperation.Combine
                result.Jobs.Add(PlanCombine(r, targets(0)))
            Case PdfOperation.MergePages
                result.Jobs.Add(PlanMerge(r, targets(0)))
            Case PdfOperation.ArrangePages
                result.Jobs.Add(PlanArrange(r, targets(0)))
            Case PdfOperation.ExtractPages
                PlanExtract(r, targets, result)
            Case PdfOperation.Encrypt
                PlanEncrypt(r, targets, result)
            Case PdfOperation.Decrypt
                PlanDecrypt(r, targets, result)
            Case PdfOperation.Rotate, PdfOperation.Stamp, PdfOperation.Compress
                PlanEachFile(r, targets, result)
        End Select
        AddExtras(r, result)
        Return result
    End Function

    ''' <summary>
    ''' Step 3's extras go into the same QPDF run as the operation itself, just before the output:
    ''' QPDF first builds the pages, then lays the background under and the stamp over them, and
    ''' encrypts the file as it writes it (each piece, for split results).
    ''' </summary>
    Private Sub AddExtras(r As OperationRequest, plan As OperationPlan)
        Dim protect = r.UsesExtraProtection
        Dim layers = r.UsesExtraLayers
        If Not protect AndAlso Not layers Then Return

        For Each job In plan.Jobs
            Dim extra As New List(Of String)
            If layers Then
                Dim pages = PageRange.Parse(r.ExtraLayers.Pages).QpdfRange
                If r.ExtraLayers.UsesBackground Then AddLayer(extra, "--underlay", r.ExtraLayers.BackgroundItem, r.ExtraLayers.BackgroundPageNumber, pages)
                If r.ExtraLayers.UsesStamp Then AddLayer(extra, "--overlay", r.ExtraLayers.StampItem, r.ExtraLayers.StampPageNumber, pages)
            End If
            If protect Then
                ' The new password replaces any the originals had (and Combine's "keep details" unlocking).
                job.Arguments.Remove("--decrypt")
                extra.AddRange(EncryptOptions(r.ExtraProtection))
                job.ReadPassword = Nothing
            End If
            Dim at = job.Arguments.LastIndexOf(OutputToken)
            job.Arguments.InsertRange(at, extra)
        Next
        If protect Then plan.ResultPassword = r.ExtraProtection.ReadPassword
    End Sub

    ''' <summary>QPDF's encryption options: AES 256-bit with the given password and limits.</summary>
    Private Function EncryptOptions(p As ProtectionSettings) As List(Of String)
        ' A file with only a permissions password opens without one, but QPDF needs
        ' both set. With only a password to open, that password also grants all rights.
        Dim owner = If(p.PermissionsPassword.Length > 0, p.PermissionsPassword, p.OpenPassword)
        Return New List(Of String) From {
            "--encrypt",
            "--user-password=" & p.OpenPassword,
            "--owner-password=" & owner,
            "--bits=256",
            "--print=" & If(p.AllowPrinting, "full", "none"),
            "--modify=" & If(p.AllowEditing, "all", "none"),
            "--extract=" & If(p.AllowCopying, "y", "n"),
            "--"
        }
    End Function

    ''' <summary>
    ''' The options Reduce Size gives QPDF. Every level repacks the file losslessly; Medium and above
    ''' also re-save photos as JPEG at lower quality, which QPDF only keeps where it is smaller.
    ''' (--decode-level=all is deliberately not used: it makes photo-heavy files several times larger.)
    ''' </summary>
    Public Function CompressOptions(level As CompressLevel) As List(Of String)
        Dim options As New List(Of String) From {
            "--object-streams=generate", "--compress-streams=y", "--recompress-flate",
            "--compression-level=9", "--remove-unreferenced-resources=yes"
        }
        Select Case level
            Case CompressLevel.Medium
                options.AddRange({"--optimize-images", "--jpeg-quality=75"})
            Case CompressLevel.High
                options.AddRange({"--optimize-images", "--jpeg-quality=50"})
            Case CompressLevel.Extreme
                options.AddRange({"--optimize-images", "--jpeg-quality=30",
                                  "--oi-min-width=32", "--oi-min-height=32", "--oi-min-area=4096",
                                  "--remove-metadata", "--remove-info", "--remove-structure", "--remove-page-labels"})
        End Select
        Return options
    End Function

    ''' <summary>Rotate, Stamp and Reduce Size: one QPDF run per PDF, each keeping any password it had.</summary>
    Private Sub PlanEachFile(r As OperationRequest, targets As List(Of OutputTarget), plan As OperationPlan)
        If r.Operation = PdfOperation.Compress Then
            plan.InputBytes = r.Items.Sum(Function(i) i.SizeBytes)
        End If

        For i = 0 To r.Items.Count - 1
            Dim item = r.Items(i)
            Dim job As New QpdfJob With {.Target = targets(i), .ReadPassword = item.Password}
            Dim args = job.Arguments
            If Not String.IsNullOrEmpty(item.Password) Then args.Add("--password=" & item.Password)
            args.Add(item.Path)

            Select Case r.Operation
                Case PdfOperation.Rotate
                    job.Description = $"Rotating {item.FileName}"
                    Dim angle = If(r.RotateAngle = -90, "-90", If(r.RotateAngle = 180, "+180", "+90"))
                    args.Add($"--rotate={angle}:{RangeOf(r.RotatePages, item)}")

                Case PdfOperation.Stamp
                    Select Case r.StampMode
                        Case StampMode.BackgroundOnly : job.Description = $"Adding a background to {item.FileName}"
                        Case StampMode.Both : job.Description = $"Adding a stamp and a background to {item.FileName}"
                        Case Else : job.Description = $"Stamping {item.FileName}"
                    End Select
                    Dim pages = RangeOf(r.StampPages, item)
                    ' QPDF can lay pages under and over in the same run.
                    If r.UsesBackground Then AddLayer(args, "--underlay", r.BackgroundItem, r.BackgroundPageNumber, pages)
                    If r.UsesStamp Then AddLayer(args, "--overlay", r.StampItem, r.StampPageNumber, pages)

                Case PdfOperation.Compress
                    job.Description = $"Reducing the size of {item.FileName}"
                    args.AddRange(CompressOptions(r.CompressLevel))
            End Select

            args.Add(OutputToken)
            plan.Jobs.Add(job)
        Next
    End Sub

    Private Function PlanCombine(r As OperationRequest, target As OutputTarget) As QpdfJob
        Dim job As New QpdfJob With {.Description = "Combining PDFs", .Target = target}
        If r.KeepFirstFileDetails Then
            ' The first file stays the base document, so its bookmarks and details survive.
            AddPrimary(job.Arguments, r.Items(0))
            job.Arguments.Add("--pages")
            job.Arguments.AddRange({".", "1-z"})
            For Each item In r.Items.Skip(1)
                AddPageSource(job.Arguments, item, "1-z")
            Next
        Else
            job.Arguments.AddRange({"--empty", "--pages"})
            For Each item In r.Items
                AddPageSource(job.Arguments, item, "1-z")
            Next
        End If
        job.Arguments.Add("--")
        job.Arguments.Add(OutputToken)
        Return job
    End Function

    Private Function PlanMerge(r As OperationRequest, target As OutputTarget) As QpdfJob
        Dim job As New QpdfJob With {.Description = "Merging pages", .Target = target}
        job.Arguments.AddRange({"--empty", "--pages"})
        For Each item In r.Items
            AddPageSource(job.Arguments, item, RangeOf(item.PagesText, item))
        Next
        job.Arguments.Add("--")
        job.Arguments.Add(OutputToken)
        Return job
    End Function

    ''' <summary>
    ''' The kept pages in their new order, in one QPDF run. Neighbouring pages from the same PDF
    ''' share one entry ("a.pdf 1-3,7 b.pdf 2 a.pdf 9"), which keeps the instructions short.
    ''' </summary>
    Private Function PlanArrange(r As OperationRequest, target As OutputTarget) As QpdfJob
        Dim job As New QpdfJob With {.Description = "Saving the arranged pages", .Target = target}
        job.Arguments.AddRange({"--empty", "--pages"})
        For Each run In GroupRuns(r.PagesToArrange())
            AddPageSource(job.Arguments, run.Item, PageList(run.Pages))
        Next
        job.Arguments.Add("--")
        job.Arguments.Add(OutputToken)
        Return job
    End Function

    ''' <summary>Splits the pages into runs of neighbours that come from the same PDF.</summary>
    Friend Function GroupRuns(pages As IList(Of PageRef)) As List(Of (Item As PdfItem, Pages As List(Of Integer)))
        Dim runs As New List(Of (Item As PdfItem, Pages As List(Of Integer)))
        For Each page In pages
            If runs.Count > 0 AndAlso runs(runs.Count - 1).Item Is page.Item Then
                runs(runs.Count - 1).Pages.Add(page.PageNumber)
            Else
                runs.Add((page.Item, New List(Of Integer) From {page.PageNumber}))
            End If
        Next
        Return runs
    End Function

    ''' <summary>Page numbers as QPDF reads them, with steps of one joined: {1,2,3,7,6} becomes "1-3,7,6".</summary>
    Friend Function PageList(pages As IList(Of Integer)) As String
        Dim parts As New List(Of String)
        Dim i = 0
        While i < pages.Count
            Dim j = i
            While j + 1 < pages.Count AndAlso pages(j + 1) = pages(j) + 1
                j += 1
            End While
            parts.Add(If(j > i, $"{pages(i)}-{pages(j)}", pages(i).ToString()))
            i = j + 1
        End While
        Return String.Join(",", parts)
    End Function

    Private Sub PlanExtract(r As OperationRequest, targets As List(Of OutputTarget), plan As OperationPlan)
        If r.ExtractOutput = ExtractOutput.OnePdf Then
            Dim job As New QpdfJob With {.Description = "Extracting pages", .Target = targets(0)}
            job.Arguments.AddRange({"--empty", "--pages"})
            For Each item In r.Items
                AddPageSource(job.Arguments, item, RangeOf(r.ExtractPages, item))
            Next
            job.Arguments.Add("--")
            job.Arguments.Add(OutputToken)
            plan.Jobs.Add(job)
            Return
        End If

        Dim splitOption = If(r.ExtractOutput = ExtractOutput.EveryNPages,
                             $"--split-pages={Math.Max(1, r.SplitEvery)}",
                             "--split-pages")
        For i = 0 To r.Items.Count - 1
            Dim item = r.Items(i)
            Dim job As New QpdfJob With {.Description = $"Extracting pages from {item.FileName}", .Target = targets(i)}
            job.Arguments.AddRange({"--empty", "--pages"})
            AddPageSource(job.Arguments, item, RangeOf(r.ExtractPages, item))
            job.Arguments.Add("--")
            job.Arguments.Add(splitOption)
            job.Arguments.Add(OutputToken)
            plan.Jobs.Add(job)
        Next
    End Sub

    Private Sub PlanEncrypt(r As OperationRequest, targets As List(Of OutputTarget), plan As OperationPlan)
        Dim protection = r.Protection()
        plan.ResultPassword = protection.ReadPassword

        For i = 0 To r.Items.Count - 1
            Dim item = r.Items(i)
            Dim job As New QpdfJob With {.Description = $"Encrypting {item.FileName}", .Target = targets(i)}
            If Not String.IsNullOrEmpty(item.Password) Then job.Arguments.Add("--password=" & item.Password)
            job.Arguments.AddRange(EncryptOptions(protection))
            job.Arguments.Add(item.Path)
            job.Arguments.Add(OutputToken)
            plan.Jobs.Add(job)
        Next
    End Sub

    ''' <summary>
    ''' An unlocked copy of each PDF: no password to open it and no limits on printing, copying or
    ''' editing. A locked PDF needs its password (asked for in the list); one with only limits
    ''' opens freely, and QPDF can take the limits off without the permissions password.
    ''' The copy is read back without a password, which proves the lock is gone.
    ''' </summary>
    Private Sub PlanDecrypt(r As OperationRequest, targets As List(Of OutputTarget), plan As OperationPlan)
        For i = 0 To r.Items.Count - 1
            Dim item = r.Items(i)
            Dim job As New QpdfJob With {.Description = $"Removing the password from {item.FileName}", .Target = targets(i)}
            If Not String.IsNullOrEmpty(item.Password) Then job.Arguments.Add("--password=" & item.Password)
            job.Arguments.Add("--decrypt")
            job.Arguments.Add(item.Path)
            job.Arguments.Add(OutputToken)
            plan.Jobs.Add(job)
        Next
    End Sub

    ''' <summary>One page of another PDF laid over or under the chosen pages, the same page every time.</summary>
    Private Sub AddLayer(args As List(Of String), kind As String, layer As PdfItem, pageNumber As Integer, pages As String)
        args.Add(kind)
        args.Add(layer.Path)
        If Not String.IsNullOrEmpty(layer.Password) Then args.Add("--password=" & layer.Password)
        args.Add("--to=" & pages)
        args.Add("--from=")
        args.Add($"--repeat={Math.Max(1, pageNumber)}")
        args.Add("--")
    End Sub

    ''' <summary>Uses a file as the base document, leaving its password protection behind.</summary>
    Private Sub AddPrimary(args As List(Of String), item As PdfItem)
        If Not String.IsNullOrEmpty(item.Password) Then args.Add("--password=" & item.Password)
        args.Add("--decrypt")
        args.Add(item.Path)
    End Sub

    Private Sub AddPageSource(args As List(Of String), item As PdfItem, range As String)
        args.Add(item.Path)
        If Not String.IsNullOrEmpty(item.Password) Then args.Add("--password=" & item.Password)
        args.Add(range)
    End Sub

    Private Function RangeOf(text As String, item As PdfItem) As String
        Return PageRange.Parse(text, item.PageCount).QpdfRange
    End Function

End Module
