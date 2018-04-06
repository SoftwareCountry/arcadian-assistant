import { IntervalType } from './calendar.model';

interface IntervalTypeConverterRule {
    readonly startHour: number;
    readonly finishHour: number;
    readonly intervalType: IntervalType;

    includes(startWorkingHour: number, finishWorkingHour: number): boolean;
}

class IntervalLeftBoundaryRule implements IntervalTypeConverterRule {
    public readonly startHour = 0;
    public readonly finishHour = 4;
    public readonly intervalType = IntervalType.IntervalLeftBoundary;

    public includes(startWorkingHour: number, finishWorkingHour: number): boolean {
        return this.startHour <= startWorkingHour && finishWorkingHour <= this.finishHour;
    }
}

class IntervalRightBoundaryRule implements IntervalTypeConverterRule {
    public readonly startHour = 4;
    public readonly finishHour = 8;
    public readonly intervalType = IntervalType.IntervalRightBoundary;

    public includes(startWorkingHour: number, finishWorkingHour: number): boolean {
        return this.startHour <= startWorkingHour && finishWorkingHour <= this.finishHour;
    }
}

class IntervalFullBoundaryRule implements IntervalTypeConverterRule {
    public readonly startHour = 0;
    public readonly finishHour = 8;
    public readonly intervalType = IntervalType.IntervalFullBoundary;

    public includes(startWorkingHour: number, finishWorkingHour: number): boolean {
        return this.startHour <= startWorkingHour
            && startWorkingHour < 4
            && finishWorkingHour > 4
            && finishWorkingHour <= this.finishHour;
    }
}

export class IntervalTypeConverter {
    private static readonly rules: IntervalTypeConverterRule[] = [
        new IntervalLeftBoundaryRule(),
        new IntervalRightBoundaryRule(),
        new IntervalFullBoundaryRule()
    ];

    public static hoursToIntervalType(startWorkingHour: number, finishWorkingHour: number): IntervalType | null {
        const rule = IntervalTypeConverter.rules.find(x => x.includes(startWorkingHour, finishWorkingHour));

        if (!rule) {
            return null;
        }

        return rule.intervalType;
    }

    public static intervalTypeToHours(intervalType: IntervalType): { startHour: number, finishHour: number } | null {
        const rule = IntervalTypeConverter.rules.find(x => x.intervalType === intervalType);

        if (!rule) {
            return null;
        }

        return rule;
    }
}