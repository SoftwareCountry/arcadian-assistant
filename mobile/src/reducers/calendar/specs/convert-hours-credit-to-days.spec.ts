import { ConvertHoursCreditToDays } from '../convert-hours-credit-to-days';

describe('convert hours credit to days', () => {

    it('should return null days, null rest, if hours are null', () => {
        const converter = new ConvertHoursCreditToDays();
        const result = converter.convert(null);

        expect(result.days).toBeNull();
        expect(result.rest).toBeNull();
    });

    it('should return 0 days and empty rest, if hours are 0', () => {
        const converter = new ConvertHoursCreditToDays();
        const result = converter.convert(0);

        expect(result.days).toBe(0);
        expect(result.rest).toBe('');
    });

    describe('hours are divisble by entire day', () => {
        it('should return entire days and empty rest', () => {
            const hours = 4 * 2;
            const converter = new ConvertHoursCreditToDays();
            const result = converter.convert(hours);

            expect(result.days).toBe(1);
            expect(result.rest).toBe('');
        });

        it('should return positive days, if hours are negative', () => {
            const hours = -(4 * 2);
            const converter = new ConvertHoursCreditToDays();
            const result = converter.convert(hours);

            expect(result.days).toBe(1);
            expect(result.rest).toBe('');
        });
    });

    describe('hours are NOT divisble by entire day', () => {
        it('should return 0 days with rest, if hours <= half day', () => {
            let hours = 1;
            const converter = new ConvertHoursCreditToDays();
            let result = converter.convert(hours);

            expect(result.days).toBe(0);
            expect(result.rest).toBe(ConvertHoursCreditToDays.fractionSymbol);

            hours = 4;
            result = converter.convert(hours);

            expect(result.days).toBe(0);
            expect(result.rest).toBe(ConvertHoursCreditToDays.fractionSymbol);
        });

        it('should return days with rest, if rest <= half day', () => {
            const rest = 2;
            const hours = 4 + 4 + rest;
            const converter = new ConvertHoursCreditToDays();
            const result = converter.convert(hours);

            expect(result.days).toBe(1);
            expect(result.rest).toBe(ConvertHoursCreditToDays.fractionSymbol);
        });

        it('should return days rounded up with empty rest, if rest > half day', () => {
            const rest = 5;
            const hours = 4 + 4 + rest;
            const converter = new ConvertHoursCreditToDays();
            const result = converter.convert(hours);

            expect(result.days).toBe(2);
            expect(result.rest).toBe('');
        });

        it('should return positive days and rest, if hours are negative', () => {
            const hours = -(4 + 4 + 2);
            const converter = new ConvertHoursCreditToDays();
            const result = converter.convert(hours);

            expect(result.days).toBe(1);
            expect(result.rest).toBe(ConvertHoursCreditToDays.fractionSymbol);
        });
    });
});