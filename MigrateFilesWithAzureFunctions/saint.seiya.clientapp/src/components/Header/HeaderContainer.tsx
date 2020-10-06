import { connect } from 'react-redux';
import { StateReducer } from '../../reducers';
import { Header } from './Header';

const mapStateToProps = (state: StateReducer) => ({
});

const mapDispatchToProps = (dispatch: any) => ({    
});

export const HeaderContainer: any = connect(
    mapStateToProps,
    mapDispatchToProps
)(Header);