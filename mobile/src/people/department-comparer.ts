interface HasAbbreviation {
    abbreviation: string;
}

export function departmentAZComparer (first: HasAbbreviation, second: HasAbbreviation) {
    if (first.abbreviation < second.abbreviation) {
        return -1;
    } else if (first.abbreviation > second.abbreviation) {
        return 1;
    }

    return 0;
}