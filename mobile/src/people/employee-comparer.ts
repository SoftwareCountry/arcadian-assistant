interface HasName {
    name: string;
}

export function employeesAZComparer(first: HasName, second: HasName) {
    if (first.name < second.name) {
        return -1;
    } else if (first.name > second.name) {
        return 1;
    }

    return 0;
}
