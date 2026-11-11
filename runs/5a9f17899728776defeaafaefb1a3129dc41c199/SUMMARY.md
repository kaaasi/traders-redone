# CI-Ergebnis `5a9f178` (dev/ap2-vga)

Zeit: 2026-11-11 07:34 UTC

| Job | Ergebnis |
|---|---|
| linux | ❌ failure |
| windows | ❌ failure |

## linux-results

| Schritt | Exit | Sekunden |
|---|---:|---:|
| hash-dos | ❌ 1 | 0 |
| export | ❌ 1 | 1 |
| viewer-screenshots | ❌ 1 | 2 |
| refemu-boot | ❌ 1 | 1 |

## windows-results

| Schritt | Exit | Sekunden |
|---|---:|---:|
| build | ✅ | 20 |
| test | ❌ 1 | 11 |
| publish-viewer | ✅ | 13 |

**Tests:** 347 bestanden, 55 fehlgeschlagen, 0 sonstige

### ❌ Traders.Tests.AssetTests.English_text_table_starts_with_known_lines
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\ENGLISCH.TXT'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.TextTable.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\TextTable.cs:line 40
   at Traders.Tests.AssetTests.English_text_table_starts_with_known_lines() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 76
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.RefEmu.RandomDifferentialTests.Minus_one_divides_by_zero_in_both
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.Tests.RefEmu.RandomDifferentialTests.Prepare(UInt32 seed) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\RandomDifferentialTests.cs:line 20
   at Traders.Tests.RefEmu.RandomDifferentialTests.Minus_one_divides_by_zero_in_both() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\RandomDifferentialTests.cs:line 92
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.CopyPageToPage_351E_matches_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.CopyPageToPage_351E_matches_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 98
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "E-ANBAU.PP", length: 40964, sha256: "bff151d3b1ef33da573c371975f5570ac3aec36c809eaf8eb1"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\E-ANBAU.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.RefEmu.BootTests.Boot_to_main_menu_is_deterministic
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.BootToMainMenu(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 50
   at Traders.Tests.RefEmu.BootTests.Boot_to_main_menu_is_deterministic() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\BootTests.cs:line 73
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "V-KAMPF.PP", length: 82692, sha256: "2b3eb3aef10c0c976e3e5da169c9406edbcc11fafb3f0ef994"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-KAMPF.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.RefEmu.MachineTests.Mz_header_of_t_exe_is_parsed
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\T.EXE'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.RefEmu.MachineTests.Mz_header_of_t_exe_is_parsed() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\MachineTests.cs:line 18
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "MASKS.PP", length: 8000, sha256: "d1f9d2d657efd842f6286c8a600aca308499315c4f196d11f1"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\MASKS.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "V-BERGE.PP", length: 82692, sha256: "ff317a990992138a641ab4200abf1210ac94524f4bab81bd3e"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-BERGE.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.Vga_atlas_starts_after_size_header
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-ANBAU.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.Vga_atlas_starts_after_size_header() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 52
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.LatchCopy_328E_matches_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.LatchCopy_328E_matches_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 169
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.Png_writer_produces_valid_signature
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-MENU.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.VgaImage.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\VgaImage.cs:line 45
   at Traders.Tests.AssetTests.Png_writer_produces_valid_signature() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 89
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.Full_screen_blit_of_a_converted_picture_shows_the_picture
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-MENU.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.VgaImage.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\VgaImage.cs:line 45
   at Traders.Tests.Vga.DriverDifferentialTests.Full_screen_blit_of_a_converted_picture_shows_the_picture() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 235
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.BlitAtlasToPageFixed_3A9A_matches_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.BlitAtlasToPageFixed_3A9A_matches_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 51
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "V-ANBAU.PP", length: 82692, sha256: "3a35e18828cbb6e50e541edad1d84b958cfd7aa8e4ee7dc034"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-ANBAU.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.Executable_is_the_analysed_version
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\T.EXE'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.Executable_is_the_analysed_version() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 11
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.State.StateTests.Initial_state_matches_initialised_data_in_TEXE
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\T.EXE'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.State.StateTests.OriginalDgroup() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\State\StateTests.cs:line 18
   at Traders.Tests.State.StateTests.Initial_state_matches_initialised_data_in_TEXE() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\State\StateTests.cs:line 156
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.FontMaskMusicTests.Asset_library_loads_everything
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-ANBAU.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.VgaImage.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\VgaImage.cs:line 45
   at Traders.Assets.AssetLibrary.<>c__DisplayClass19_0.<Load>b__1(ValueTuple`3 p) in D:\a\traders-redone\traders-redone\src\Traders.Assets\AssetLibrary.cs:line 26
   at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector, IEqualityComparer`1 comparer)
   at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector)
   at Traders.Assets.AssetLibrary.Load(String dir) in D:\a\traders-redone\traders-redone\src\Traders.Assets\AssetLibrary.cs:line 26
   at Traders.Tests.FontMaskMusicTests.Asset_library_loads_everything() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 74
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* 
```
### ❌ Traders.Tests.State.StateTests.Text_pointer_table_and_station_names_are_where_documented
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\T.EXE'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.State.StateTests.OriginalDgroup() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\State\StateTests.cs:line 18
   at Traders.Tests.State.StateTests.Text_pointer_table_and_station_names_are_where_documented() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\State\StateTests.cs:line 181
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.State.StateTests.Level_presets_in_TEXE_are_as_documented
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\T.EXE'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.State.StateTests.OriginalDgroup() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\State\StateTests.cs:line 18
   at Traders.Tests.State.StateTests.Level_presets_in_TEXE_are_as_documented() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\State\StateTests.cs:line 164
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.SetDisplayStart_30E9_matches_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.SetDisplayStart_30E9_matches_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 206
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "FONTS.PP", length: 2150, sha256: "08b98cf0d0669dd5788171d1edee4e8f48abc24208da9de10e"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\FONTS.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.CopyAtlasToAtlas_3449_matches_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.CopyAtlasToAtlas_3449_matches_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 82
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.FontMaskMusicTests.Music_header_orders_and_pattern_count(file: "TRADINTR.CMF", orders: 30, patterns: 24)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\TRADINTR.CMF'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.MusicModule.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\MusicModule.cs:line 46
   at Traders.Tests.FontMaskMusicTests.Music_header_orders_and_pattern_count(String file, Int32 orders, Int32 patterns) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 65
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeDirectByRefWithFewArgs(Object obj, Span`1 copyOfArgs, BindingFlags invokeAttr)
```
### ❌ Traders.Tests.AssetTests.Rocket_atlas_has_sprites_below_visible_area
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-RAKETE.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.VgaImage.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\VgaImage.cs:line 45
   at Traders.Tests.AssetTests.Rocket_atlas_has_sprites_below_visible_area() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 65
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.MaskFill_3CE8_matches_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.MaskFill_3CE8_matches_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 130
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "V-FATMIK.PP", length: 82692, sha256: "e5a4914b940bd4a046a2cd99dc05117349b987d15f1fe218cb"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-FATMIK.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.RefEmu.RandomDifferentialTests.Zero_reseeds_from_borland_time
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.Tests.RefEmu.RandomDifferentialTests.Prepare(UInt32 seed) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\RandomDifferentialTests.cs:line 20
   at Traders.Tests.RefEmu.RandomDifferentialTests.Zero_reseeds_from_borland_time() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\RandomDifferentialTests.cs:line 102
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.FontMaskMusicTests.Font_layout_covers_file_exactly
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\FONTS.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.FontSet.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\BitmapFont.cs:line 77
   at Traders.Tests.FontMaskMusicTests.get_Fonts() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 7
   at Traders.Tests.FontMaskMusicTests.Font_layout_covers_file_exactly() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 14
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.FontMaskMusicTests.Large_exclamation_mark_matches_original_bits
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\FONTS.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.FontSet.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\BitmapFont.cs:line 77
   at Traders.Tests.FontMaskMusicTests.get_Fonts() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 7
   at Traders.Tests.FontMaskMusicTests.Large_exclamation_mark_matches_original_bits() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 33
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.Planar_row_conversion_3079_matches_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.Planar_row_conversion_3079_matches_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 218
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.RefEmu.BootTests.T_exe_boots_into_vga_graphics
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.Tests.RefEmu.BootTests.T_exe_boots_into_vga_graphics() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\BootTests.cs:line 17
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "E-RAKETE.PP", length: 40964, sha256: "c6b6759c9f0f52963cff7b6341fc7cd5edc398d9020fffbcc9"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\E-RAKETE.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.FontMaskMusicTests.Masks_are_a_full_screen_bitplane_with_content
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\MASKS.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.MaskImage.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\MaskImage.cs:line 28
   at Traders.Tests.FontMaskMusicTests.Masks_are_a_full_screen_bitplane_with_content() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 51
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "V-RAKETE.PP", length: 82692, sha256: "15b753dde823a8ec8ed949c7cb2dd587b5220870b1bd459b01"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-RAKETE.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.FontMaskMusicTests.Space_is_empty_in_both_fonts
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\FONTS.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.FontSet.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\BitmapFont.cs:line 77
   at Traders.Tests.FontMaskMusicTests.get_Fonts() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 7
   at Traders.Tests.FontMaskMusicTests.Space_is_empty_in_both_fonts() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 22
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.RefEmu.RandomDifferentialTests.Sequence_matches_port_for_many_arguments
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.Tests.RefEmu.RandomDifferentialTests.Prepare(UInt32 seed) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\RandomDifferentialTests.cs:line 20
   at Traders.Tests.RefEmu.RandomDifferentialTests.Sequence_matches_port_for_many_arguments() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\RandomDifferentialTests.cs:line 52
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "V-ABEND.PP", length: 82692, sha256: "20dd898d7f71b5d054d266ae8ed494368e1d880b02e8fab23e"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-ABEND.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.RefEmu.MachineTests.Loading_applies_relocations_and_builds_psp
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.Tests.RefEmu.MachineTests.Loading_applies_relocations_and_builds_psp() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\MachineTests.cs:line 32
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.FontMaskMusicTests.Music_header_orders_and_pattern_count(file: "TRADMENU.CMF", orders: 11, patterns: 9)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\TRADMENU.CMF'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.MusicModule.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\MusicModule.cs:line 46
   at Traders.Tests.FontMaskMusicTests.Music_header_orders_and_pattern_count(String file, Int32 orders, Int32 patterns) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 65
   at InvokeStub_FontMaskMusicTests.Music_header_orders_and_pattern_count(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.MaskedBlit_35DF_matches_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.MaskedBlit_35DF_matches_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 113
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "V-BHANDE.PP", length: 82692, sha256: "91f5827c9c9756b5b9693e22e39bc35a502f0948417a4bca7d"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-BHANDE.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "V-ENDE.PP", length: 82692, sha256: "82b509239c2c62c2b05dbbff2671fc34e55dce7d9b93192fc1"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-ENDE.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.RefEmu.RandomDifferentialTests.Arbitrary_start_seeds_match
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.Tests.RefEmu.RandomDifferentialTests.Prepare(UInt32 seed) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\RandomDifferentialTests.cs:line 20
   at Traders.Tests.RefEmu.RandomDifferentialTests.Arbitrary_start_seeds_match() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\RandomDifferentialTests.cs:line 78
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.OrAtlasToPage_3386_matches_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.OrAtlasToPage_3386_matches_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 66
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.All_vga_pictures_load_with_full_atlas
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-ANBAU.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.VgaImage.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\VgaImage.cs:line 45
   at Traders.Tests.AssetTests.All_vga_pictures_load_with_full_atlas() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 43
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.FontMaskMusicTests.Small_hash_sign_matches_original_bits
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\FONTS.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.FontSet.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\BitmapFont.cs:line 77
   at Traders.Tests.FontMaskMusicTests.get_Fonts() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 7
   at Traders.Tests.FontMaskMusicTests.Small_hash_sign_matches_original_bits() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 43
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.SetPixel_373B_and_GetPixel_3764_match_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.SetPixel_373B_and_GetPixel_3764_match_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 146
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.DrawText_05DD_matches_original_for_both_fonts
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.DrawText_05DD_matches_original_for_both_fonts() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 184
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.BlitAtlasToPage_32D8_matches_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.BlitAtlasToPage_32D8_matches_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 35
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "V-MENU.PP", length: 82692, sha256: "19a072f12d1c1506f8a031f637c05378cd052d034d17ac2aed"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-MENU.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at InvokeStub_AssetTests.PowerPacker_decrunches_like_reference(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(file: "V-AHANDE.PP", length: 82692, sha256: "f50fed22651d614341e87b9e2cd9ff708a2ec3a23b7fd85d48"···)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\V-AHANDE.PP'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Tests.AssetTests.PowerPacker_decrunches_like_reference(String file, Int32 length, String sha256) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\AssetTests.cs:line 33
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeDirectByRefWithFewArgs(Object obj, Span`1 copyOfArgs, BindingFlags invokeAttr)
```
### ❌ Traders.Tests.RefEmu.RandomDifferentialTests.Initial_seed_in_the_data_segment_is_one
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.Tests.RefEmu.RandomDifferentialTests.Initial_seed_in_the_data_segment_is_one() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\RefEmu\RandomDifferentialTests.cs:line 44
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.FontMaskMusicTests.Music_header_orders_and_pattern_count(file: "TRAD_END.CMF", orders: 28, patterns: 12)
```
System.IO.DirectoryNotFoundException : Could not find a part of the path 'D:\a\traders-redone\traders-redone\original\dos\TRAD_END.CMF'.
at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize)
   at System.IO.File.ReadAllBytes(String path)
   at Traders.Assets.MusicModule.Load(String path) in D:\a\traders-redone\traders-redone\src\Traders.Assets\MusicModule.cs:line 46
   at Traders.Tests.FontMaskMusicTests.Music_header_orders_and_pattern_count(String file, Int32 orders, Int32 patterns) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\FontMaskMusicTests.cs:line 65
   at InvokeStub_FontMaskMusicTests.Music_header_orders_and_pattern_count(Object, Span`1)
   at System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
### ❌ Traders.Tests.Vga.DriverDifferentialTests.FillRect_31A6_matches_original
```
System.IO.FileNotFoundException : T.EXE
at Traders.RefEmu.Machine.PcMachine.LoadProgram(String exeName, String commandTail) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\Machine\PcMachine.Dos.cs:line 46
   at Traders.RefEmu.OriginalProgram.LoadOnly(String dosDirectory, MachineOptions options) in D:\a\traders-redone\traders-redone\src\Traders.RefEmu\OriginalProgram.cs:line 36
   at Traders.Tests.Vga.DriverHarness..ctor(Int32 seed, Boolean realFonts) in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverHarness.cs:line 26
   at Traders.Tests.Vga.DriverDifferentialTests.FillRect_31A6_matches_original() in D:\a\traders-redone\traders-redone\tests\Traders.Tests\Vga\DriverDifferentialTests.cs:line 20
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
```
<details><summary>Bestandene Tests</summary>

- Traders.Tests.Int16MathTests.Addition_wraps_like_8086(a: -32768, b: -1, expected: 32767)
- Traders.Tests.Int16MathTests.Addition_wraps_like_8086(a: 32767, b: 1, expected: -32768)
- Traders.Tests.Int16MathTests.Division_truncates_toward_zero(a: -7, b: 2, q: -3, r: -1)
- Traders.Tests.Int16MathTests.Division_truncates_toward_zero(a: 7, b: -2, q: -3, r: 1)
- Traders.Tests.OriginalRandomTests.First_step_from_initial_seed_gives_pi_digits
- Traders.Tests.OriginalRandomTests.Missing_clock_input_is_an_error_not_a_silent_default
- Traders.Tests.OriginalRandomTests.Sequence_matches_reference
- Traders.Tests.OriginalRandomTests.Zero_argument_reseeds_from_clock_and_returns_zero
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "00")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "01")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "02")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "03")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "04")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "05")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "06")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "07")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "08")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "09")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "0A")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "0B")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "0C")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "0D")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "0E")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "10")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "11")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "12")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "13")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "14")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "15")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "16")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "17")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "18")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "19")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "1A")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "1B")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "1C")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "1D")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "1E")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "1F")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "20")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "21")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "22")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "23")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "24")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "25")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "27")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "28")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "29")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "2A")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "2B")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "2C")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "2D")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "2F")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "30")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "31")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "32")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "33")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "34")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "35")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "37")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "38")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "39")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "3A")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "3B")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "3C")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "3D")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "3F")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "40")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "41")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "42")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "43")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "44")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "45")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "46")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "47")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "48")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "49")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "4A")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "4B")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "4C")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "4D")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "4E")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "4F")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "50")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "51")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "52")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "53")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "54")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "55")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "56")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "57")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "58")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "59")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "5A")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "5B")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "5C")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "5D")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "5E")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "5F")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "60")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "61")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "62")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "68")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "69")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "6A")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "6B")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "6C")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "6D")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "6E")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "6F")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "70")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "71")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "72")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "73")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "74")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "75")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "76")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "77")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "78")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "79")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "7A")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "7B")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "7C")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "7D")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "7E")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "7F")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "80.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "80.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "80.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "80.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "80.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "80.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "80.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "80.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "81.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "81.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "81.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "81.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "81.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "81.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "81.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "81.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "82.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "82.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "82.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "82.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "82.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "82.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "82.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "82.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "83.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "83.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "83.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "83.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "83.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "83.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "83.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "83.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "84")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "85")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "86")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "87")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "88")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "89")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "8A")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "8B")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "8C")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "8D")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "8E")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "8F")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "90")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "91")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "92")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "93")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "94")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "95")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "96")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "97")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "98")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "99")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "9A")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "9B")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "9C")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "9D")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "9E")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "9F")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "A0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "A1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "A2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "A3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "A4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "A5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "A6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "A7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "A8")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "A9")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "AA")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "AB")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "AC")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "AD")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "AE")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "AF")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "B0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "B1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "B2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "B3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "B4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "B5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "B6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "B7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "B8")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "B9")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "BA")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "BB")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "BC")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "BD")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "BE")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "BF")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C0.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C0.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C0.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C0.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C0.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C0.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C0.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C0.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C1.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C1.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C1.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C1.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C1.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C1.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C1.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C1.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C8")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "C9")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "CA")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "CB")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "CC")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "CD")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "CE")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "CF")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D0.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D0.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D0.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D0.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D0.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D0.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D0.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D0.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D1.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D1.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D1.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D1.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D1.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D1.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D1.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D1.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D2.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D2.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D2.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D2.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D2.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D2.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D2.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D2.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D3.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D3.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D3.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D3.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D3.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D3.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D3.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D3.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "D8")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "E0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "E1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "E2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "E3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "E4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "E5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "E6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "E7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "E8")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "E9")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "EA")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "EB")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "EC")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "ED")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "EE")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "EF")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F6.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F6.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F6.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F6.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F6.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F6.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F6.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F6.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F7.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F7.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F7.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F7.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F7.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F7.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F7.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F7.7")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F8")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "F9")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FA")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FB")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FC")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FD")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FE.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FE.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FF.0")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FF.1")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FF.2")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FF.3")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FF.4")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FF.5")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Instruction_form_matches_real_286(group: "FF.6")
- Traders.Tests.RefEmu.Cpu286SingleStepTests.Test_data_is_complete
- Traders.Tests.RefEmu.MachineTests.Default_vectors_point_to_host_stubs_and_int64_is_optional
- Traders.Tests.RefEmu.MachineTests.Vga_chained_mode_13h_is_linear
- Traders.Tests.RefEmu.MachineTests.Vga_unchained_write_and_render_round_trip
- Traders.Tests.RefEmu.MachineTests.Vga_write_mode_1_copies_latches
- Traders.Tests.State.StateTests.Dgroup_mapping_round_trips
- Traders.Tests.State.StateTests.Every_save_slot_is_covered_by_the_state_model
- Traders.Tests.State.StateTests.Load_rejects_files_of_wrong_size
- Traders.Tests.State.StateTests.Save_and_load_round_trip_keeps_unsaved_fields
- Traders.Tests.State.StateTests.Save_extends_bytes_and_words_like_the_original
- Traders.Tests.State.StateTests.Save_record_has_522_dwords_and_fits_the_file
- Traders.Tests.State.StateTests.Save_sites_match_the_original_save_routine
- Traders.Tests.State.StateTests.State_serialises_to_json_and_back

</details>
