import React, { Component } from 'react';
import { FlatList, View } from 'react-native';
import { StyledText } from '../override/styled-text';
import { companyDepartmentLevelPeople } from './styles';
import { CompanyDepartmentsLevelPeopleTouchable } from './company-departments-level-people-touchable';
import { employeesAZComparer } from './employee-comparer';
import { Avatar } from './avatar';
import { Employee } from '../reducers/organization/employee.model';
import { Optional } from 'types';

//============================================================================
interface CompanyDepartmentsLevelPeopleProps {
    chief: Optional<Employee>;
    employees: Employee[];
    onPressEmployee: (employee: Employee) => void;
}

//============================================================================
export class CompanyDepartmentsLevelPeople extends Component<CompanyDepartmentsLevelPeopleProps> {

    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelPeopleProps) {
        return !this.areEmployeesEqual(this.props.employees, nextProps.employees)
            || this.props.onPressEmployee !== nextProps.onPressEmployee;
    }

    //----------------------------------------------------------------------------
    public render() {
        const { employees, chief } = this.props;

        let employeesToRender: Employee[] = employees.sort(employeesAZComparer);

        if (chief) {
            employeesToRender = employeesToRender.filter(employee => employee.employeeId !== chief.employeeId);
        }

        return (
            <FlatList
                style={companyDepartmentLevelPeople.list}
                data={employeesToRender}
                keyExtractor={this.keyExtractor}
                renderItem={this.renderItem}
                scrollEnabled={true}
                refreshing={false}
            />
        );
    }

    //----------------------------------------------------------------------------
    private areEmployeesEqual(a: Employee[], b: Employee[]) {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        if (a.length !== b.length) {
            return false;
        }

        for (let i = 0; i < a.length; i++) {
            if (!a[i].equals(b[i])) {
                return false;
            }
        }

        return true;
    }

    //----------------------------------------------------------------------------
    private keyExtractor = (item: Employee): string => item.employeeId;

    //----------------------------------------------------------------------------
    private renderItem = ({ item }: { item: Employee }) => {
        return (
            <CompanyDepartmentsLevelPeopleTouchable onPress={this.props.onPressEmployee} employee={item}>
                <View style={companyDepartmentLevelPeople.listItemAvatar}>
                    <Avatar photoUrl={item.photoUrl}/>
                </View>
                <View style={companyDepartmentLevelPeople.listItemContent}>
                    <StyledText style={companyDepartmentLevelPeople.listItemName}>
                        {item.name}
                    </StyledText>
                    <StyledText style={companyDepartmentLevelPeople.listItemPosition}>
                        {item.position}
                    </StyledText>
                </View>
            </CompanyDepartmentsLevelPeopleTouchable>
        );
    };
}
