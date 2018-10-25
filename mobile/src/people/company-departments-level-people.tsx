import React, { Component } from 'react';
import { View, FlatList } from 'react-native';
import { StyledText } from '../override/styled-text';
import { companyDepartmentLevelPeople } from './styles';
import { CompanyDepartmentsLevelPeopleTouchable } from './company-departments-level-people-touchable';
import { employeesAZComparer } from './employee-comparer';
import { Avatar } from './avatar';
import { EmployeeMap } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';

interface CompanyDepartmentsLevelPeopleProps {
    employeesById: EmployeeMap;
    onPressEmployee: (employee: Employee) => void;
}

export class CompanyDepartmentsLevelPeople extends Component<CompanyDepartmentsLevelPeopleProps> {
    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelPeopleProps) {
        return !this.isEmployeesEqual(this.props.employeesById, nextProps.employeesById) 
            || this.props.onPressEmployee !== nextProps.onPressEmployee;
    }

    public render() {
        const employees: Employee[] = this.props.employeesById
            .toArray()
            .sort((a, b) => employeesAZComparer(a, b));

        return (
            <FlatList
                style={companyDepartmentLevelPeople.list}
                data={employees}
                keyExtractor={this.keyExtractor}
                renderItem={this.renderItem}
                scrollEnabled={true}
                refreshing={false}
            />
        );
    }

    private isEmployeesEqual(a: EmployeeMap, b: EmployeeMap) {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }
        
        if (a.equals(b)) {
            return true;
        }

        const aArray = a.toArray();
        const bArray = b.toArray();

        if (aArray.length !== bArray.length) {
            return false;
        }

        for (let i = 0; i < aArray.length; i++) {
            if (!aArray[i].equals(bArray[i])) {
                return false;
            }
        }        

        return true;
    }    

    private keyExtractor = (item: Employee): string => item.employeeId;

    private renderItem = ({ item }: { item: Employee }) => {
        return (
            <CompanyDepartmentsLevelPeopleTouchable onPress={this.props.onPressEmployee} employee={item}>
                <View style={companyDepartmentLevelPeople.listItemAvator}>
                    <Avatar photoUrl={item.photoUrl} />
                </View>
                <View style={companyDepartmentLevelPeople.listItemContent}>
                    <StyledText style={companyDepartmentLevelPeople.listItemName}>
                        {`${item.name}, `}
                    </StyledText>
                    <StyledText style={companyDepartmentLevelPeople.listItemPosition}>
                        {item.position}
                    </StyledText>
                </View>
            </CompanyDepartmentsLevelPeopleTouchable>
        );
    }
}