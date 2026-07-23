import {ValidationError} from "../../core/logic/ValidateError";

export class TimeRange {
    private readonly startTime: Date;
    private readonly endTime: Date;

    private constructor(startTime: Date, endTime: Date) {
        this.startTime = startTime;
        this.endTime = endTime;
    }

    static create(startTime: Date, endTime: Date) {
        this.validate(startTime, endTime);
        return new TimeRange(startTime, endTime);
    }
    static validate(startTime: Date, endTime: Date) {
        if (!startTime || !endTime) {
            throw new ValidationError('Start time and end time are required.');
        }
        if (!(startTime instanceof Date) || isNaN(startTime.getTime())) {
            throw new ValidationError('Invalid Start time.', 'startTime');
        }
        if (!(endTime instanceof Date) || isNaN(endTime.getTime())) {
            throw new ValidationError('Invalid End time.', 'endTime');
        }
        if (endTime <= startTime) {
            throw new ValidationError('End time must be after start time', 'endTime');
        }
    }

    getStartTime(): Date {
        return this.startTime;
    }

    getEndTime(): Date {
        return this.endTime;
    }

    getDurationInMinutes(): number {
        return (this.endTime.getTime() - this.startTime.getTime()) / (1000 * 60);
    }

    getDurationInHours(): number {
        return this.getDurationInMinutes() / 60;
    }

    overlaps(other: TimeRange): boolean {
        return this.startTime < other.endTime && this.endTime > other.startTime;
    }
}