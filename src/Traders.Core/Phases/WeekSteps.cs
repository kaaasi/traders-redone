using Traders.Core.Ai;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Startup;
using Traders.Core.Week;

namespace Traders.Core.Phases;

/// <summary>
/// All ported call steps of the week loop 1777:01F8 for the <see cref="PhaseMachine"/> (docs/arbeitspakete.md R5, lane anbau
/// part 3): the screenless rule steps of <see cref="WeekRuleSteps"/>, the week report (call 3, 0825:000A(1), lane bilanz),
/// the guard of the catastrophes (call 4, 111D:0009, pending after week 2 when switched on), the week planning of the computer
/// players (calls 6 and 21, 11A3:0000(0/1), lane planung) and the planting phase (call 8, 0609:0004 with the computer turn
/// of lane anbau-ki and the wait loop of lane bilanz). The first call step still pending is call 9 (week events).
/// </summary>
public sealed class WeekSteps
{
    /// <summary>Call number of the week report 0825:000A(1).</summary>
    public const int ReportCall = 3;

    /// <summary>Call number of the planting phase 0609:0004.</summary>
    public const int PlantingCall = 8;

    /// <param name="planting">Output of the planting phase (scene and sound) and its controls.</param>
    /// <param name="files">Lengths of the picture files (picture table of 186C:0002).</param>
    /// <param name="devices">Control devices besides the key table; keyboard only if null.</param>
    /// <param name="catastrophes">Handler for call 4 beyond its guard (null = pending, the machine stops there).</param>
    public WeekSteps(Action<FieldEvent> planting, Func<int, ImageFileLengths> files, IControlDevices? devices = null,
        IWeekStepHandler? catastrophes = null)
    {
        devices ??= KeyboardOnlyDevices.Instance;
        Report = new ReportStep(devices);
        var report = new ReportPictureStep(Report, files);
        Planting = new PlantingStep(new PlantingHost(planting, devices), files);
        Handlers = WeekRuleSteps.Handlers();
        Handlers[ReportCall] = report;
        Handlers[CatastropheGuardStep.Call] = new CatastropheGuardStep(catastrophes);
        Handlers[WeekPlanningStep.BeforePlantingCall] = WeekPlanningStep.ForCall(WeekPlanningStep.BeforePlantingCall);
        Handlers[PlantingCall] = Planting;
        Handlers[WeekPlanningStep.BeforeTradeCall] = WeekPlanningStep.ForCall(WeekPlanningStep.BeforeTradeCall);
    }

    /// <summary>
    /// The report of call 3; <see cref="ReportStep.Report"/> holds the running report and its <see cref="WeekEvent"/>s. The
    /// handler bound to call 3 wraps it and records the picture load in the picture table.
    /// </summary>
    public ReportStep Report { get; }

    /// <summary>Call 8; its events go to the output given to the constructor.</summary>
    public PlantingStep Planting { get; }

    /// <summary>Handlers by call number, for <see cref="PhaseMachine"/> (and the week machine of <see cref="ProgramMachine"/>).</summary>
    public Dictionary<int, IWeekStepHandler> Handlers { get; }

    /// <summary>Call numbers with a handler that run in every week (call 4 only up to week 2 or without catastrophes).</summary>
    public static IReadOnlyList<int> ScreenCalls { get; } =
        [ReportCall, CatastropheGuardStep.Call, WeekPlanningStep.BeforePlantingCall, PlantingCall, WeekPlanningStep.BeforeTradeCall];

    /// <summary>
    /// Call 3 with the core part of the picture load 186C:0002(DS:08CE) of the report: the picture table entry and DS:09C8
    /// (<see cref="ImageTable.RecordLoad"/>), as <see cref="PlantingStep"/> does for the planting phase. <see cref="Report"/>
    /// emits the load as its first event (0825:000A before the first wait), so it is recorded at the start of the step.
    /// </summary>
    private sealed class ReportPictureStep(ReportStep report, Func<int, ImageFileLengths> files) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context)
        {
            var outcome = report.Begin(context);
            // [ORIGINAL] 186C:0002 – called from 0825:000A; the picture table entry and DS:09C8 (ImageTable.RecordLoad)
            foreach (var e in report.Report!.Events.Where(e => e.Kind == WeekEventKind.LoadPicture))
                ImageTable.RecordLoad(context.State, e.A, files);
            return outcome;
        }

        public StepOutcome Tick(StepContext context) => report.Tick(context);
    }
}
